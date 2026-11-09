namespace RetroGames.Repository;

// O MySQL recusou um valor repetido numa coluna UNIQUE (erro 1062).
// O Service captura e transforma no erro de negócio correspondente.
public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
