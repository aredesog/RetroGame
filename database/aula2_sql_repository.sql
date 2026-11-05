-- =====================================================================
-- RetroGames · aula2_sql_repository.sql
-- SQL de cada método do Repository, testado no Workbench antes de ir
-- para o código (Aula 2). Os @valores simulam os parâmetros do C#.
-- Execute com Ctrl+Enter, um comando por vez.
-- =====================================================================

USE retrogame;

-- =====================================================================
-- ETAPA 3 · PlataformaRepository (tb_plataforma)
-- =====================================================================

-- Listar: todas, ordenadas por nome
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
ORDER BY no_plataforma;

-- Buscar por id (existe: 1 linha; inexistente: 0 linhas)
SET @id = 1;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

SET @id = 999;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- Existe com nome
-- a) inclusão (sem id a ignorar): esperado 1 linha
SET @nome = 'Super Nintendo', @idIgnorar = NULL;
SELECT 1 FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar)
LIMIT 1;

-- b) alteração da própria plataforma 1: esperado 0 linhas (não acusa ela mesma)
SET @nome = 'Super Nintendo', @idIgnorar = 1;
SELECT 1 FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar)
LIMIT 1;

-- Possui jogos
SET @id = 1;   -- Super Nintendo: esperado 1 linha
SELECT 1 FROM tb_jogo WHERE co_plataforma = @id LIMIT 1;

SET @id = 7;   -- Sega Saturn: esperado 0 linhas
SELECT 1 FROM tb_jogo WHERE co_plataforma = @id LIMIT 1;

-- Inserir (esperado: 1 row affected) e o id gerado
SET @nome = 'Atari 2600', @fabricante = 'Atari', @anoLancamento = 1977;
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES (@nome, @fabricante, @anoLancamento);
SET @id = LAST_INSERT_ID();
SELECT @id AS id_gerado;

-- Alterar a plataforma recém-inserida
SET @nome = 'Atari 2600 VCS', @fabricante = 'Atari', @anoLancamento = 1977;
UPDATE tb_plataforma
SET no_plataforma = @nome,
    no_fabricante = @fabricante,
    nu_ano_lancamento = @anoLancamento
WHERE co_seq_plataforma = @id;

-- Excluir a plataforma recém-inserida (limpa o teste)
DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id;

-- Conferência: esperado 0 linhas
SELECT * FROM tb_plataforma WHERE no_plataforma LIKE 'Atari%';


-- =====================================================================
-- ETAPA 4 · JogoRepository (tb_jogo)
-- O WHERE é o mesmo na contagem e na página. Filtro NULL = não aplicado.
-- =====================================================================

-- Listar · sem filtro, página 1 (5 por página): esperado total 23, 5 itens
SET @busca = NULL, @idPlataforma = NULL, @incluirInativos = FALSE, @limite = 5, @deslocamento = 0;

SELECT COUNT(*) AS total FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma);

SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
       qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
ORDER BY no_jogo
LIMIT 5 OFFSET 0;   -- no Workbench, LIMIT/OFFSET aceita só número fixo; no C# vão como @limite e @deslocamento

-- Listar · com inativos: esperado total 24 (aparece Punch-Out!!)
SET @incluirInativos = TRUE;
SELECT COUNT(*) AS total FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma);
SET @incluirInativos = FALSE;

-- Listar · busca "Super", página 2: esperado total 6, 1 item (Super Metroid)
SET @busca = 'Super';
SELECT COUNT(*) AS total FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma);

SELECT co_seq_jogo, no_jogo FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
ORDER BY no_jogo
LIMIT 5 OFFSET 5;
SET @busca = NULL;

-- Listar · plataforma 3 (Nintendo 64): esperado total 4
SET @idPlataforma = 3;
SELECT co_seq_jogo, no_jogo FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
ORDER BY no_jogo
LIMIT 5 OFFSET 0;
SET @idPlataforma = NULL;

-- Listar · lista de ids (o Service manda os jogos de um gênero): esperado 3 itens
-- No C# cada id vira um parâmetro: IN (@idJogo0, @idJogo1, @idJogo2)
SELECT co_seq_jogo, no_jogo FROM tb_jogo
WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
  AND co_seq_jogo IN (1, 12, 22)
ORDER BY no_jogo
LIMIT 5 OFFSET 0;

-- Buscar por id
SET @id = 13;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
       qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
FROM tb_jogo
WHERE co_seq_jogo = @id;

-- Inserir (jogo de teste)
SET @nome = 'Jogo Teste Repository', @descricao = NULL, @anoLancamento = 1999,
    @preco = 50.00, @estoque = 1, @ativo = TRUE, @dataCadastro = UTC_TIMESTAMP(),
    @idPlataforma = 1, @idCondicaoJogo = 2;
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (@nome, @descricao, @anoLancamento, @preco, @estoque,
     @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo);
SET @idTeste = LAST_INSERT_ID();
SELECT @idTeste AS id_gerado;

-- Alterar
SET @nome = 'Jogo Teste Repository (alterado)', @preco = 60.00;
UPDATE tb_jogo
SET no_jogo = @nome,
    ds_jogo = @descricao,
    nu_ano_lancamento = @anoLancamento,
    vl_preco = @preco,
    qt_estoque = @estoque,
    co_plataforma = @idPlataforma,
    co_condicao_jogo = @idCondicaoJogo
WHERE co_seq_jogo = @idTeste;

-- Desativar (exclusão lógica)
UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @idTeste;
SELECT co_seq_jogo, no_jogo, vl_preco, st_ativo FROM tb_jogo WHERE co_seq_jogo = @idTeste;


-- =====================================================================
-- ETAPA 4 · JogoGeneroRepository (tb_jogo_genero)
-- =====================================================================

-- Listar todos: esperado 33 linhas
SELECT co_jogo, co_genero FROM tb_jogo_genero;

-- Listar por jogos (uma consulta só): esperado 5 linhas
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (2, 13);

-- Inserir vínculos do jogo de teste (um INSERT, várias linhas)
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idTeste, 2), (@idTeste, 3);
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo = @idTeste;

-- Remover por jogo
DELETE FROM tb_jogo_genero WHERE co_jogo = @idTeste;

-- Limpeza do teste (só aqui, no Workbench: o Repository nunca apaga jogo)
DELETE FROM tb_jogo WHERE co_seq_jogo = @idTeste;
SELECT COUNT(*) AS deve_ser_zero FROM tb_jogo WHERE no_jogo LIKE 'Jogo Teste%';
