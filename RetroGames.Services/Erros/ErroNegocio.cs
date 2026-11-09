using System.Net;

namespace RetroGames.Services.Erros;

// Um erro do catálogo. O status é só um valor: quem responde HTTP é a API (aula 4).
public sealed record ErroNegocio(int Codigo, string Mensagem, HttpStatusCode Status);
