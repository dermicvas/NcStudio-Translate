# Contribuindo para o NcStudio Translate

Obrigado pelo seu interesse em contribuir! Este documento fornece diretrizes para contribuir com o projeto.

## 🚀 Como Começar

1. **Fork** o repositório
2. **Clone** seu fork localmente
3. **Crie uma branch** para sua feature ou correção
4. **Faça suas alterações**
5. **Teste** suas alterações
6. **Commit** com mensagens claras
7. **Push** para seu fork
8. **Abra um Pull Request**

## 📝 Padrões de Código

### Estilo de Código

- Use **PascalCase** para nomes de classes, métodos e propriedades públicas
- Use **camelCase** para variáveis locais e parâmetros
- Use **_camelCase** (underscore prefix) para campos privados
- Sempre declare o modificador de acesso explicitamente
- Uma classe por arquivo (exceto classes aninhadas)

### Estrutura de Arquivos

```
src/
├── Forms/        # Formulários WinForms
├── Helpers/      # Classes utilitárias
├── Models/       # Classes de dados/modelo
└── Resources/    # Ícones, imagens, etc.
```

### Comentários

- Use comentários XML (`///`) para documentar classes, métodos e propriedades públicas
- Escreva comentários em português ou inglês (consistente com o arquivo)
- Evite comentários óbvios

### Exemplo

```csharp
namespace NcStudioTranslate.Models
{
    /// <summary>
    /// Representa uma entrada de recurso no arquivo .resx.
    /// </summary>
    internal sealed class ResourceEntry
    {
        private string _key = string.Empty;

        /// <summary>
        /// Chave única do recurso.
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value ?? string.Empty;
        }
    }
}
```

## 🧪 Testando

Antes de submeter um PR:

1. Compile o projeto sem erros
2. Execute a aplicação e teste manualmente as funcionalidades afetadas
3. Verifique se não há warnings desnecessários

```bash
dotnet build -c Release
dotnet run
```

## 📋 Pull Requests

### Checklist

- [ ] O código compila sem erros
- [ ] O código segue os padrões de estilo do projeto
- [ ] As alterações foram testadas
- [ ] A documentação foi atualizada (se necessário)
- [ ] A descrição do PR explica claramente as mudanças

### Título do PR

Use um título descritivo seguindo o padrão:

- `feat: adiciona suporte a novos idiomas`
- `fix: corrige erro ao carregar arquivos grandes`
- `docs: atualiza README com novas instruções`
- `refactor: reorganiza estrutura de pastas`

### Descrição

Inclua:
- O que foi alterado
- Por que foi alterado
- Como testar as alterações

## 🐛 Reportando Bugs

Ao reportar um bug, inclua:

1. **Título** claro e descritivo
2. **Passos** para reproduzir o problema
3. **Comportamento esperado** vs. real
4. **Screenshots** (se aplicável)
5. **Ambiente**: versão do Windows, versão do .NET

## 💡 Sugerindo Features

Ao sugerir uma nova feature:

1. Verifique se já não foi sugerida
2. Descreva claramente a feature
3. Explique o caso de uso
4. Considere possíveis implementações

## 📜 Licença

Ao contribuir, você concorda que suas contribuições poderão ser incorporadas ao projeto sob os termos descritos em [LICENSE](LICENSE).

Em particular:
- Você pode modificar o código localmente para preparar um Pull Request neste repositório.
- Ao submeter um Pull Request (ou qualquer contribuição com código), você concede aos mantenedores permissão para usar, modificar e distribuir sua contribuição como parte do projeto.

## ❓ Dúvidas?

Abra uma [Discussion](../../discussions) para perguntas gerais ou use [Issues](../../issues) para bugs e features.

---

Obrigado por contribuir! 🎉
