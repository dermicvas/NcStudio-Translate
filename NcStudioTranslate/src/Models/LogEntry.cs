using System;

namespace NcStudioTranslate.Models
{
    /// <summary>
    /// Representa uma entrada no log de alterações.
    /// </summary>
    internal sealed class LogEntry
    {
        /// <summary>
        /// Data e hora da alteração.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Nome do arquivo alterado.
        /// </summary>
        public string File { get; set; } = string.Empty;

        /// <summary>
        /// Chave do recurso alterado.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Valor original antes da alteração.
        /// </summary>
        public string OriginalValue { get; set; } = string.Empty;

        /// <summary>
        /// Novo valor inserido.
        /// </summary>
        public string InsertedValue { get; set; } = string.Empty;

        /// <summary>
        /// Origem da alteração (Edição manual, Criação de tradução, etc.).
        /// </summary>
        public string Origin { get; set; } = string.Empty;
    }
}
