-- =====================================================================
-- RetroGames · 03_consultas_e_testes.sql
-- Aula 1, etapa 5: consultas do catálogo e testes de integridade.
--
-- Observação: aqui usamos JOIN porque é um exercício de SQL no banco.
-- Na API, o Repository lê uma tabela por consulta e a composição dos
-- dados é feita no Service, com LINQ (Espec. 3.1).
-- =====================================================================

USE retrogame;

-- =====================================================================
-- PARTE 1 · CONSULTAS
-- =====================================================================

-- 1. Catálogo: jogos ativos com plataforma e condição, ordenados por nome
SELECT
    j.co_seq_jogo,
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j
INNER JOIN tb_plataforma    p ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo c ON c.co_condicao_jogo  = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. Gêneros de um jogo informado (troque o valor de @co_jogo)
SET @co_jogo = 13;   -- The Legend of Zelda: Ocarina of Time

SELECT g.co_seq_genero, g.no_genero
FROM tb_jogo_genero jg
INNER JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @co_jogo
ORDER BY g.no_genero;

-- 3. Jogos de um gênero informado (troque o valor de @co_genero)
SET @co_genero = 1;  -- Plataforma

SELECT j.co_seq_jogo, j.no_jogo, j.st_ativo
FROM tb_jogo_genero jg
INNER JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = @co_genero
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo plataformas sem jogos
--    LEFT JOIN mantém a plataforma mesmo sem jogo; COUNT(coluna) ignora NULL e dá 0.
--    Esperado: Sega Saturn com 0.
SELECT
    p.co_seq_plataforma,
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, 5 itens por página
--    OFFSET = (página - 1) * 5. Com 'Super' há 6 jogos ativos:
--    página 1 traz 5, página 2 traz 1.
SET @busca = 'Super';

-- Página 1
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE
  AND no_jogo LIKE CONCAT('%', @busca, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 0;

-- Página 2
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE
  AND no_jogo LIKE CONCAT('%', @busca, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 5;


-- =====================================================================
-- PARTE 2 · TESTES DE INTEGRIDADE
-- Execute UM comando por vez (Ctrl+Enter com o cursor no comando),
-- confira a mensagem no painel Output e anote o resultado obtido.
-- =====================================================================

-- Teste 1: jogo com co_plataforma inexistente
-- Esperado: Erro 1452 (chave estrangeira)
-- Obtido:   ____
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste FK', 1990, 10.00, 1, UTC_TIMESTAMP(), 999, 2);

-- Teste 2: plataforma com nome já cadastrado
-- Esperado: Erro 1062 (UNIQUE)
-- Obtido:   ____
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);

-- Teste 3: excluir plataforma que possui jogos
-- Esperado: Erro 1451 (chave estrangeira)
-- Obtido:   ____
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;

-- Teste 4: jogo com no_jogo nulo
-- Esperado: Erro 1048 (NOT NULL)
-- Obtido:   ____
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (NULL, 1990, 10.00, 1, UTC_TIMESTAMP(), 1, 2);

-- Teste 5: jogo com vl_preco igual a 0
-- Esperado: Aceito. A regra "preço > 0" fica no Service (RN06), não no banco.
-- Obtido:   ____
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste Preço Zero', 1990, 0.00, 1, UTC_TIMESTAMP(), 1, 2);

SET @co_jogo_teste = LAST_INSERT_ID();   -- guarde o id logo depois do Teste 5


-- =====================================================================
-- PARTE 3 · LIMPEZA DOS DADOS DE TESTE
-- Só o Teste 5 grava algo; os outros falham e não deixam dados.
-- =====================================================================

DELETE FROM tb_jogo WHERE co_seq_jogo = @co_jogo_teste;

-- Conferência: esperado nenhuma linha
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE no_jogo LIKE 'Teste%';
