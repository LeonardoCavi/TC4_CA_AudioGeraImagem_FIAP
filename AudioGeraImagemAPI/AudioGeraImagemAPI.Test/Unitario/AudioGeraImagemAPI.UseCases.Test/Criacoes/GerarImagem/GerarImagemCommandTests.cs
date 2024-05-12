using AudioGeraImagemAPI.UseCases.Criacoes.GerarImagem;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace AudioGeraImagemAPI.Test.Unitario.AudioGeraImagemAPI.UseCases.Test.Criacoes.GerarImagem
{
    public class GerarImagemCommandTests
    {
        [Fact]
        public void Valido_Teste_Sucesso()
        {
            // Arrange
            var descricao = "teste";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3")
            {
                Headers = new HeaderDictionary()
                {
                    {"Content-Disposition", "form-data; name=\"file\"; filename=\"dummy.mp3\""},
                    {"Content-Type", "audio/mpeg"}
                }
            };

            var command = new GerarImagemCommand(descricao, file);

            // Act
            var ehValido = command.Valido();

            // Assert
            Assert.True(ehValido);
        }

        [Fact]
        public void Valido_Teste_Falha_ContentType()
        {
            // Arrange
            var descricao = "teste";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3")
            {
                Headers = new HeaderDictionary()
                {
                    {"Content-Disposition", "form-data; name=\"file\"; filename=\"dummy.mp3\""},
                    {"Content-Type", "audio/wav"}
                }
            };

            var command = new GerarImagemCommand(descricao, file);

            // Act
            var ehValido = command.Valido();

            // Assert
            Assert.False(ehValido);
        }

        [Fact]
        public void Valido_Teste_Falha_Descricao()
        {
            // Arrange
            var descricao = "teste......................................................................................................................................................................................................................................................................";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.mp3")
            {
                Headers = new HeaderDictionary()
                {
                    {"Content-Disposition", "form-data; name=\"file\"; filename=\"dummy.mp3\""},
                    {"Content-Type", "audio/wav"}
                }
            };

            var command = new GerarImagemCommand(descricao, file);

            // Act
            var ehValido = command.Valido();

            // Assert
            Assert.False(ehValido);
        }
    }
}
