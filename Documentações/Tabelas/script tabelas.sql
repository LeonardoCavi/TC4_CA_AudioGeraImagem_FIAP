CREATE DATABASE GeraImagem;
USE GeraImagem;

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Comandos] (
    [Id] uniqueidentifier NOT NULL,
    [UrlAudio] VARCHAR(MAX) NULL,
    [Transcricao] VARCHAR(MAX) NULL,
    [UrlImagem] VARCHAR(MAX) NULL,
    [InstanteCriacao] DATETIME2 NOT NULL,
    [InstanteAtualizacao] DATETIME2 NOT NULL,
    CONSTRAINT [PK_Comandos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ProcessamentoComandos] (
    [Id] uniqueidentifier NOT NULL,
    [Estado] VARCHAR(20) NOT NULL,
    [InstanteCriacao] DATETIME2 NOT NULL,
    [MensagemErro] VARCHAR(256) NULL,
    [ComandoId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ProcessamentoComandos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProcessamentoComandos_Comandos_ComandoId] FOREIGN KEY ([ComandoId]) REFERENCES [Comandos] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProcessamentoComandos_ComandoId] ON [ProcessamentoComandos] ([ComandoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240315113826_Inicial', N'7.0.16');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Comandos] ADD [Descricao] VARCHAR(256) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240315151800_Adiciona descricao', N'7.0.16');
GO

COMMIT;
GO
