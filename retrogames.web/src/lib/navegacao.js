export function obterDestinoInterno(voltarPara) {
  if (!voltarPara?.startsWith('/') || voltarPara.startsWith('//') || voltarPara.includes('\\')) {
    return '/'
  }

  const destino = new URL(voltarPara, window.location.origin)
  return destino.origin === window.location.origin ? voltarPara : '/'
}
