-- =====================================================================
-- RetroGames · 01_criar_tabelas.sql
-- Cria o banco e todas as tabelas do dicionário de dados (Espec. 4.1 a 4.3).
--
-- Regras seguidas:
--   * O banco guarda só a estrutura: PK, FK (comportamento padrão),
--     tipos, NOT NULL e UNIQUE.
--   * Sem trigger, function, CHECK, ON DELETE CASCADE ou ON DELETE SET NULL.
--   * Tabelas de domínio (tb_condicao_jogo, tb_perfil) usam chave fixa,
--     sem AUTO_INCREMENT.
--
-- Para recriar do zero: DROP DATABASE retrogame; e execute este script.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS retrogame
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE retrogame;

-- ---------------------------------------------------------------------
-- Tabelas de domínio (chave fixa)
-- ---------------------------------------------------------------------

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo  INT          NOT NULL,
    ds_condicao_jogo  VARCHAR(50)  NOT NULL,
    CONSTRAINT pk_condicao_jogo PRIMARY KEY (co_condicao_jogo),
    CONSTRAINT uk_condicao_jogo_descricao UNIQUE (ds_condicao_jogo)
) ENGINE = InnoDB;

CREATE TABLE tb_perfil (
    co_perfil  INT          NOT NULL,
    ds_perfil  VARCHAR(50)  NOT NULL,
    CONSTRAINT pk_perfil PRIMARY KEY (co_perfil),
    CONSTRAINT uk_perfil_descricao UNIQUE (ds_perfil)
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- Tabelas de cadastro
-- ---------------------------------------------------------------------

CREATE TABLE tb_plataforma (
    co_seq_plataforma  INT           NOT NULL AUTO_INCREMENT,
    no_plataforma      VARCHAR(100)  NOT NULL,
    no_fabricante      VARCHAR(100)  NOT NULL,
    nu_ano_lancamento  INT           NOT NULL,
    CONSTRAINT pk_plataforma PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uk_plataforma_nome UNIQUE (no_plataforma)
) ENGINE = InnoDB;

CREATE TABLE tb_genero (
    co_seq_genero  INT          NOT NULL AUTO_INCREMENT,
    no_genero      VARCHAR(50)  NOT NULL,
    CONSTRAINT pk_genero PRIMARY KEY (co_seq_genero),
    CONSTRAINT uk_genero_nome UNIQUE (no_genero)
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- Tabela principal
-- ---------------------------------------------------------------------

CREATE TABLE tb_jogo (
    co_seq_jogo        INT            NOT NULL AUTO_INCREMENT,
    no_jogo            VARCHAR(150)   NOT NULL,
    ds_jogo            VARCHAR(1000)  NULL,            -- única coluna opcional do módulo
    nu_ano_lancamento  INT            NOT NULL,
    vl_preco           DECIMAL(10,2)  NOT NULL,
    qt_estoque         INT            NOT NULL,
    st_ativo           BOOLEAN        NOT NULL DEFAULT TRUE,
    dt_cadastro        DATETIME       NOT NULL,        -- data e hora em UTC
    co_plataforma      INT            NOT NULL,
    co_condicao_jogo   INT            NOT NULL,
    CONSTRAINT pk_jogo PRIMARY KEY (co_seq_jogo),
    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma) REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao_jogo
        FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo (co_condicao_jogo),
    INDEX idx_jogo_nome (no_jogo)
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- Tabela associativa (N:N entre jogo e gênero)
-- ---------------------------------------------------------------------

CREATE TABLE tb_jogo_genero (
    co_jogo    INT  NOT NULL,
    co_genero  INT  NOT NULL,
    CONSTRAINT pk_jogo_genero PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo) REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero) REFERENCES tb_genero (co_seq_genero)
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- Usuários
-- ---------------------------------------------------------------------

CREATE TABLE tb_usuario (
    co_seq_usuario  INT           NOT NULL AUTO_INCREMENT,
    no_usuario      VARCHAR(100)  NOT NULL,
    ds_email        VARCHAR(150)  NOT NULL,
    ds_senha_hash   VARCHAR(255)  NOT NULL,
    st_ativo        BOOLEAN       NOT NULL DEFAULT TRUE,
    dt_cadastro     DATETIME      NOT NULL,            -- data e hora em UTC
    co_perfil       INT           NOT NULL,
    CONSTRAINT pk_usuario PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uk_usuario_email UNIQUE (ds_email),
    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil) REFERENCES tb_perfil (co_perfil)
) ENGINE = InnoDB;

-- Conferência: deve listar as 7 tabelas
SHOW TABLES;
