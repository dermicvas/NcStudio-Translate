using NcStudioTranslate.Forms;

namespace NcStudioTranslate
{
    internal static class Program
    {
        private static readonly string ErrorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configura handlers de exceção global
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                LogError("Erro fatal na inicialização", ex);
                MessageBox.Show(
                    $"Erro fatal ao iniciar a aplicação:\n\n{ex.Message}\n\nDetalhes salvos em: {ErrorLogPath}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogError("Exceção de thread da UI", e.Exception);
            MessageBox.Show(
                $"Erro não tratado:\n\n{e.Exception.Message}\n\nDetalhes salvos em: {ErrorLogPath}",
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            LogError("Exceção não tratada do AppDomain", ex);
            MessageBox.Show(
                $"Erro crítico:\n\n{ex?.Message ?? "Erro desconhecido"}\n\nDetalhes salvos em: {ErrorLogPath}",
                "Erro Crítico",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void LogError(string context, Exception? ex)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logEntry = $"""
                    ============================================================
                    [{timestamp}] {context}
                    ------------------------------------------------------------
                    Mensagem: {ex?.Message ?? "N/A"}
                    Tipo: {ex?.GetType().FullName ?? "N/A"}
                    StackTrace:
                    {ex?.StackTrace ?? "N/A"}
                    
                    InnerException: {ex?.InnerException?.Message ?? "N/A"}
                    InnerException StackTrace:
                    {ex?.InnerException?.StackTrace ?? "N/A"}
                    ============================================================

                    """;

                // Insere no início do arquivo para manter ordem decrescente (mais recente primeiro)
                var existingContent = File.Exists(ErrorLogPath) ? File.ReadAllText(ErrorLogPath) : string.Empty;
                File.WriteAllText(ErrorLogPath, logEntry + existingContent);
            }
            catch
            {
                // Se não conseguir logar, ignora para não causar mais erros
            }
        }
    }
}