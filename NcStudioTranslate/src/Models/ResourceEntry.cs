namespace NcStudioTranslate.Models
{
    /// <summary>
    /// Representa uma entrada de recurso no arquivo .resx.
    /// </summary>
    internal sealed class ResourceEntry
    {
        /// <summary>
        /// Chave única do recurso.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Valor atual (editável) do recurso.
        /// </summary>
        public string CurrentValue { get; set; } = string.Empty;

        /// <summary>
        /// Valor original de referência (do arquivo en-US).
        /// </summary>
        public string OriginalValue { get; set; } = string.Empty;
    }
}
