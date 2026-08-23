using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using simulador_de_banco.Application.Interface.IServices;

namespace simulador_de_banco.Controller
{
    public class TransacaoController : ControllerBase
    {
        private readonly IContaCorrenteService _contaCorrenteService;

        public TransacaoController(IContaCorrenteService contaCorrenteService)
        {
            _contaCorrenteService = contaCorrenteService;
        }

        [HttpPost]
        // GET: HomeController
        public async Task<ActionResult> Transacao(int idContaOrigem, int idContaDestino,decimal valor, CancellationToken cancellationToken)
        {
            return Ok(await _contaCorrenteService.TransferirAsync(idContaOrigem, idContaDestino, valor, cancellationToken));
        }
    }
}
