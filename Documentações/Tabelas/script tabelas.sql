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

CREATE TABLE [Criacao] (
    [Id] uniqueidentifier NOT NULL,
    [Descricao] VARCHAR(256) NULL,
    [UrlAudio] VARCHAR(MAX) NULL,
    [Transcricao] VARCHAR(MAX) NULL,
    [UrlImagem] VARCHAR(MAX) NULL,
    [InstanteCriacao] DATETIME2 NOT NULL,
    [InstanteAtualizacao] DATETIME2 NOT NULL,
    CONSTRAINT [PK_Criacao] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ProcessamentosCriacao] (
    [Id] uniqueidentifier NOT NULL,
    [Estado] VARCHAR(20) NOT NULL,
    [InstanteCriacao] DATETIME2 NOT NULL,
    [MensagemErro] VARCHAR(MAX) NULL,
    [CriacaoId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ProcessamentosCriacao] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProcessamentosCriacao_Criacao_CriacaoId] FOREIGN KEY ([CriacaoId]) REFERENCES [Criacao] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProcessamentosCriacao_CriacaoId] ON [ProcessamentosCriacao] ([CriacaoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240510215734_Inicial', N'7.0.16');
GO

COMMIT;
GO


