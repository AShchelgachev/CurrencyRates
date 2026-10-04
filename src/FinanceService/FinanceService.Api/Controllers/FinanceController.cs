using FinanceService.Api.Extensions;
using FinanceService.Application.Commands.AddFavorite;
using FinanceService.Application.Commands.RemoveFavorite;
using FinanceService.Application.Dto;
using FinanceService.Application.Queries.GetCurrencies;
using FinanceService.Application.Queries.GetUserRates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/finance")]
public class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("rates")]
    public async Task<ActionResult<IReadOnlyList<CurrencyRateDto>>> GetMyRates(CancellationToken cancellationToken)
    {
        var rates = await _mediator.Send(new GetUserRatesQuery(User.GetUserId()), cancellationToken);
        return Ok(rates);
    }

    [HttpGet("currencies")]
    public async Task<ActionResult<IReadOnlyList<CurrencyRateDto>>> GetCurrencies(CancellationToken cancellationToken)
    {
        var currencies = await _mediator.Send(new GetCurrenciesQuery(), cancellationToken);
        return Ok(currencies);
    }

    [HttpPost("favorites/{currencyId:int}")]
    public async Task<IActionResult> AddFavorite(int currencyId, CancellationToken cancellationToken)
    {
        var added = await _mediator.Send(new AddFavoriteCurrencyCommand(User.GetUserId(), currencyId), cancellationToken);
        return added ? NoContent() : NotFound();
    }

    [HttpDelete("favorites/{currencyId:int}")]
    public async Task<IActionResult> RemoveFavorite(int currencyId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveFavoriteCurrencyCommand(User.GetUserId(), currencyId), cancellationToken);
        return NoContent();
    }
}
