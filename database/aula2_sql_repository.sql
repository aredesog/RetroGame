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
