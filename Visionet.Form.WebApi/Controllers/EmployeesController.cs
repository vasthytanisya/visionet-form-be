using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Visionet.Form.Commons.RequestHandlers.Employees;
using Visionet.Form.Commons.Validations.Employees;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;

namespace Visionet.Form.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("list")]
        public async Task<ActionResult<ListEmployeeResponse>> Get()
        {
            var response = await _mediator.Send(new ListEmployeeRequest());
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<GetEmployeeResponse>> Get([FromQuery]GetEmployeeRequest model, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }

        [HttpGet("skill")]
        public async Task<ActionResult<List<GetSkillResponse>>> GetSkill()
        {
            var response = await _mediator.Send(new GetSkillRequest());
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<CreateEmployeeResponse>> Post([FromBody]CreateEmployeeRequest model, [FromServices] IValidator<CreateEmployeeRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(model, cancellationToken) ?? throw new InvalidOperationException("Failed to validate data");

            if (validationResult.IsValid == false)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<UpdateEmployeeResponse>> Post([FromBody] UpdateEmployeeRequest model, [FromServices] IValidator<UpdateEmployeeRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(model, cancellationToken) ?? throw new InvalidOperationException("Failed to validate data");

            if (validationResult.IsValid == false)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<DeleteEmployeeResponse>> Delete([FromQuery] DeleteEmployeeRequest model, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }
    }
}
