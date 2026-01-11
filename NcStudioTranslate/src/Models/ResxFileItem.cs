namespace NcStudioTranslate.Models
{
    /// <summary>
    /// Representa um item de arquivo .resx na lista de arquivos.
    /// </summary>
    internal sealed class ResxFileItem
    {
        public ResxFileItem(string baseName)
        {
            BaseName = baseName;
        }

        /// <summary>
        /// Nome base do arquivo (sem extensão de idioma).
        /// </summary>
        public string BaseName { get; }

        /// <summary>
        /// Caminho completo do arquivo em chinês (.zh-CN.resx).
        /// </summary>
        public string? ZhPath { get; set; }

        /// <summary>
        /// Caminho completo do arquivo em inglês (.en-US.resx).
        /// </summary>
        public string? EnPath { get; set; }

        /// <summary>
        /// Caminho do arquivo principal (alias para ZhPath).
        /// </summary>
        public string? FilePath => ZhPath;

        /// <summary>
        /// Nome de exibição na lista.
        /// </summary>
        public string DisplayName => BaseName;
    }
}
