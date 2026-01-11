namespace NcStudioTranslate.Models
{
    /// <summary>
    /// Configurações persistentes da aplicação.
    /// </summary>
    internal sealed class AppSettings
    {
        /// <summary>
        /// Última pasta selecionada pelo usuário.
        /// </summary>
        public string? LastFolder { get; set; }

        /// <summary>
        /// Nível de zoom da interface (70-200%).
        /// </summary>
        public int ZoomPercent { get; set; } = 100;

        /// <summary>
        /// Posição do divisor do SplitContainer.
        /// </summary>
        public int SplitterDistance { get; set; } = 0;

        /// <summary>
        /// Indica se a coluna "Chave" deve ser exibida.
        /// </summary>
        public bool ShowKeyColumn { get; set; } = false;

        /// <summary>
        /// Indica se a primeira execução já foi concluída (para exibir tutorial).
        /// </summary>
        public bool FirstRunCompleted { get; set; } = false;
    }
}
