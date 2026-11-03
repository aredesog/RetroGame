-- =====================================================================
-- RetroGames · 02_dados_exemplo.sql
-- Dados de exemplo (Aula 1, etapa 4). Execute logo depois do 01.
--
--   * Condições e perfis com chaves fixas.
--   * 7 plataformas (a Sega Saturn fica sem jogos, para a contagem da etapa 5).
--   * 9 gêneros.
--   * 24 jogos reais, cada um com 1 a 3 gêneros.
--     - Punch-Out!! está inativo (st_ativo = FALSE).
--     - Street Fighter II Turbo está com estoque zero.
--     - 6 jogos têm "Super" no nome, para testar a 2ª página da busca.
--   * Nenhum usuário: o administrador será criado pela API.
-- =====================================================================

USE retrogame;

-- ---------------------------------------------------------------------
-- Domínios (valores iguais às constantes CondicaoJogoIds e PerfilIds)
-- ---------------------------------------------------------------------

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- ---------------------------------------------------------------------
-- Plataformas
-- ---------------------------------------------------------------------

INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    (1, 'Super Nintendo',                'Nintendo', 1990),
    (2, 'Mega Drive',                    'Sega',     1988),
    (3, 'Nintendo 64',                   'Nintendo', 1996),
    (4, 'PlayStation',                   'Sony',     1994),
    (5, 'Game Boy',                      'Nintendo', 1989),
    (6, 'Nintendo Entertainment System', 'Nintendo', 1983),
    (7, 'Sega Saturn',                   'Sega',     1994);   -- sem jogos

-- ---------------------------------------------------------------------
-- Gêneros
-- ---------------------------------------------------------------------

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
    (1, 'Plataforma'),
    (2, 'Ação'),
    (3, 'Aventura'),
    (4, 'RPG'),
    (5, 'Corrida'),
    (6, 'Luta'),
    (7, 'Esporte'),
    (8, 'Puzzle'),
    (9, 'Tiro');

-- ---------------------------------------------------------------------
-- Jogos
-- Condição: 1 = Novo, 2 = Usado, 3 = Recondicionado
-- ---------------------------------------------------------------------

INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    -- Super Nintendo
    ( 1, 'Super Mario World', 'Mario e Yoshi atravessam a Dinosaur Land para salvar a princesa.', 1990, 249.90, 5, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 2, 'The Legend of Zelda: A Link to the Past', 'Link viaja entre o Mundo da Luz e o Mundo das Trevas.', 1991, 299.90, 3, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 3, 'Donkey Kong Country', 'Donkey e Diddy recuperam o estoque de bananas roubado.', 1994, 199.90, 4, TRUE,  UTC_TIMESTAMP(), 1, 3),
    ( 4, 'Super Metroid', 'Samus Aran explora o planeta Zebes.', 1994, 349.90, 2, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 5, 'Chrono Trigger', 'RPG de viagem no tempo da Square.', 1995, 899.90, 1, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 6, 'Street Fighter II Turbo', 'Versão turbo do clássico de luta da Capcom.', 1993, 149.90, 0, TRUE,  UTC_TIMESTAMP(), 1, 2),   -- estoque zero
    ( 7, 'International Superstar Soccer Deluxe', NULL, 1995, 99.90, 3, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 8, 'Super Mario Kart', 'Primeira corrida de kart da série Mario.', 1992, 219.90, 2, TRUE,  UTC_TIMESTAMP(), 1, 3),
    -- Mega Drive
    ( 9, 'Sonic the Hedgehog', 'A estreia do ouriço azul da Sega.', 1991, 129.90, 6, TRUE,  UTC_TIMESTAMP(), 2, 2),
    (10, 'Streets of Rage 2', 'Beat ''em up com trilha de Yuzo Koshiro.', 1992, 179.90, 3, TRUE,  UTC_TIMESTAMP(), 2, 3),
    (11, 'Mortal Kombat', NULL, 1993, 119.90, 2, TRUE,  UTC_TIMESTAMP(), 2, 2),
    -- Nintendo 64
    (12, 'Super Mario 64', 'A primeira aventura 3D de Mario.', 1996, 279.90, 4, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (13, 'The Legend of Zelda: Ocarina of Time', 'Link viaja no tempo para enfrentar Ganondorf.', 1998, 329.90, 3, TRUE,  UTC_TIMESTAMP(), 3, 3),
    (14, 'Mario Kart 64', NULL, 1996, 229.90, 5, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (15, 'GoldenEye 007', 'Tiro em primeira pessoa baseado no filme de James Bond.', 1997, 199.90, 2, TRUE,  UTC_TIMESTAMP(), 3, 2),
    -- PlayStation
    (16, 'Final Fantasy VII', 'Cloud e a AVALANCHE enfrentam a Shinra.', 1997, 189.90, 3, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (17, 'Crash Bandicoot', NULL, 1996, 99.90, 7, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (18, 'Metal Gear Solid', 'Solid Snake se infiltra em Shadow Moses.', 1998, 159.90, 2, TRUE,  UTC_TIMESTAMP(), 4, 3),
    (19, 'Gran Turismo', 'Simulador de corrida da Polyphony Digital.', 1997, 89.90, 4, TRUE,  UTC_TIMESTAMP(), 4, 2),
    -- Game Boy
    (20, 'Tetris', 'O quebra-cabeça de blocos que acompanhava o Game Boy.', 1989, 79.90, 8, TRUE,  UTC_TIMESTAMP(), 5, 1),
    (21, 'Pokémon Red', NULL, 1996, 249.90, 3, TRUE,  UTC_TIMESTAMP(), 5, 3),
    -- Nintendo Entertainment System
    (22, 'Super Mario Bros.', 'O clássico de plataforma de 1985.', 1985, 199.90, 4, TRUE,  UTC_TIMESTAMP(), 6, 2),
    (23, 'The Legend of Zelda', NULL, 1986, 229.90, 1, TRUE,  UTC_TIMESTAMP(), 6, 2),
    (24, 'Punch-Out!!', 'Little Mac sobe no ranking do boxe.', 1987, 149.90, 2, FALSE, UTC_TIMESTAMP(), 6, 2);  -- inativo

-- ---------------------------------------------------------------------
-- Associação jogo × gênero (1 a 3 gêneros por jogo)
-- Gêneros: 1 Plataforma, 2 Ação, 3 Aventura, 4 RPG, 5 Corrida,
--          6 Luta, 7 Esporte, 8 Puzzle, 9 Tiro
-- ---------------------------------------------------------------------

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    ( 1, 1),
    ( 2, 3), ( 2, 2),
    ( 3, 1),
    ( 4, 2), ( 4, 3),
    ( 5, 4),
    ( 6, 6),
    ( 7, 7),
    ( 8, 5),
    ( 9, 1),
    (10, 2), (10, 6),
    (11, 6),
    (12, 1), (12, 3),
    (13, 3), (13, 2), (13, 4),
    (14, 5),
    (15, 9), (15, 2),
    (16, 4),
    (17, 1),
    (18, 2), (18, 3),
    (19, 5),
    (20, 8),
    (21, 4),
    (22, 1),
    (23, 3),
    (24, 7), (24, 6);

-- ---------------------------------------------------------------------
-- Conferências (Confira da etapa 4)
-- ---------------------------------------------------------------------

-- Quantidades: esperado >= 6 plataformas, >= 8 gêneros, >= 20 jogos
SELECT
    (SELECT COUNT(*) FROM tb_plataforma) AS qt_plataformas,
    (SELECT COUNT(*) FROM tb_genero)     AS qt_generos,
    (SELECT COUNT(*) FROM tb_jogo)       AS qt_jogos;

-- Jogos sem gênero: esperado nenhuma linha
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo j
LEFT JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_jogo IS NULL;

-- Jogo inativo e jogo com estoque zero: esperado Punch-Out!! e Street Fighter II Turbo
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0;

-- Usuários: esperado 0
SELECT COUNT(*) AS qt_usuarios FROM tb_usuario;
