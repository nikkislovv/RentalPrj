using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentCommandApi.Commands;
using RentCommandApi.DTOs;
using RentCommandApi.Queries;

namespace RentCommandApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RentsController(
            IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> NewRentAsync(
            NewRentDto newRentDto,
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var car = await _mediator.Send(new GetCarQuery { Id = newRentDto.CarId }, cancellationToken);

            if (car == null)
                return NotFound(new { message = $"There is no car with such id:{newRentDto.CarId}" });

            if (car.IsAvailable == false)
                return BadRequest(new { message = $"The car with id:{newRentDto.CarId} is not available" });

            if (await _mediator.Send(new CheckRentedCarQuery { Id = newRentDto.CarId, Start = newRentDto.Start, Finish = newRentDto.Finish }, cancellationToken))
                return BadRequest(new { message = $"The car with id:{newRentDto.CarId} is already rented in this period!" });

            await _mediator.Send(new NewRentCommand
            {
                Id = id,
                Start = newRentDto.Start,
                Finish = newRentDto.Finish,
                CarId = newRentDto.CarId,
                UserId = User.FindFirst(u => u.Type == "Id").Value,
                DayCost = car.RentPrice
            }, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new
            {
                Id = id,
            });
        }

        [HttpPut("Start/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StartRentAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new StartRentCommand { Id = id }, cancellationToken);

            return Ok(new
            {
                Id = id,
                Message = "The rent start request completed successfully!"
            });
        }

        [HttpPut("Extend/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> ExtendRentAsync(
            Guid id,
            ExtendRentDto extendRentDto,
            CancellationToken cancellationToken)
        {
            var car = await _mediator.Send(new GetCarQuery { Id = extendRentDto.CarId }, cancellationToken);

            if (car == null)
                return NotFound(new { message = $"There is no car with such id:{extendRentDto.CarId}" });

            if (car.IsAvailable == false)
                return BadRequest(new { message = $"The car with id:{extendRentDto.CarId} is not available" });

            if (await _mediator.Send(new CheckRentedCarQuery { Id = extendRentDto.CarId, Finish = extendRentDto.Finish }, cancellationToken))
                return BadRequest(new { message = $"The car with id:{extendRentDto.CarId} is already rented in this period!" });

            await _mediator.Send(new ExtendRentCommand
            {
                Id = id,
                Finish = extendRentDto.Finish,
                CarId = extendRentDto.CarId,
                DayCost = car.RentPrice
            }, cancellationToken);

            return Ok(new
            {
                Id = id,
                Message = "The rent extend request completed successfully!"
            });
        }

        [HttpPut("Finish/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> FinishRentAsync(
           Guid id,
           CancellationToken cancellationToken)
        {
            await _mediator.Send(new FinishRentCommand { Id = id }, cancellationToken);

            return Ok(new
            {
                Id = id,
                Message = "The rent finish request completed successfully!"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRentAsync(
           Guid id,
           CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteRentCommand { Id = id }, cancellationToken);

            return Ok(new
            {
                Id = id,
                Message = "The rent delete request completed successfully!"
            });
        }

    }
}
