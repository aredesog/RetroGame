using System.Net;

namespace RetroGames.Services.Erros;

/// <summary>
/// Um erro de negócio do catálogo: código, mensagem e o status HTTP com que a API vai responder.
/// HttpStatusCode é só um valor (System.Net); o Service não conhece controller nem HttpContext.
/// </summary>
public sealed record ErroNegocio(int Codigo, string Mensagem, HttpStatusCode Status);
