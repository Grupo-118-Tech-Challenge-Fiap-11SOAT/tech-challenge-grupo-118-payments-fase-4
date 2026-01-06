using FluentAssertions;
using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Domain.Entities;
using Payments.Infra.Persistence;
using Payments.Tests.Bdd.Context;
using Reqnroll;

namespace Payments.Tests.Bdd.StepsDefinitions;

[Binding]
public class PaymentSteps
{
    private readonly CreatePaymentUseCase _createPaymentUseCase;
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentScenarioContext _scenarioContext;

    public PaymentSteps(
        CreatePaymentUseCase createPaymentUseCase,
        IPaymentRepository paymentRepository,
        PaymentScenarioContext scenarioContext)
    {
        _createPaymentUseCase = createPaymentUseCase;
        _paymentRepository = paymentRepository;
        _scenarioContext = scenarioContext;
    }

    [Given(@"que eu tenho um pedido com ID ""(.*)""")]
    public void GivenQueEuTenhoUmPedidoComID(string orderId)
    {
        _scenarioContext.Request = new CreatePaymentRequest
        {
            OrderId = orderId,
            Value = 100.50m
        };
    }

    [Given(@"que eu tenho um pedido com ID ""(.*)"" e valor de (.*)")]
    public void GivenQueEuTenhoUmPedidoComIDEValorDe(string orderId, decimal value)
    {
        _scenarioContext.Request = new CreatePaymentRequest
        {
            OrderId = orderId,
            Value = value
        };
    }

    [Given(@"que já existe um pagamento para o pedido ""(.*)""")]
    public async Task GivenQueJaExisteUmPagamentoParaOPedido(string orderId)
    {
        var existingPayment = Payment.Create(orderId, 150.00m, Domain.Enums.PaymentProvider.MercadoPago);
        existingPayment.SetUserPaymentCode("existing-qr-code-data");
        
        await _paymentRepository.AddAsync(existingPayment);
        await _paymentRepository.SaveChangesAsync();
    }

    [When(@"eu solicito a criação do pagamento")]
    public async Task WhenEuSolicitoACriacaoDoPagamento()
    {
        try
        {
            var result = await _createPaymentUseCase.ExecuteAsync(_scenarioContext.Request!);
            _scenarioContext.Response = result.Response;
            _scenarioContext.WasCreated = result.Created;
        }
        catch (Exception ex)
        {
            _scenarioContext.Exception = ex;
        }
    }

    [Then(@"o pagamento deve ser criado com sucesso")]
    public void ThenOPagamentoDeveSerCriadoComSucesso()
    {
        _scenarioContext.Exception.Should().BeNull();
        _scenarioContext.Response.Should().NotBeNull();
        _scenarioContext.WasCreated.Should().BeTrue();
    }

    [Then(@"o pagamento deve conter um código QR")]
    public void ThenOPagamentoDeveConterUmCodigoQR()
    {
        _scenarioContext.Response!.UserPaymentCode.Should().NotBeNullOrEmpty();
        _scenarioContext.Response.UserPaymentCode.Should().Contain("00020126");
    }

    [Then(@"o status do pagamento deve ser ""(.*)""")]
    public void ThenOStatusDoPagamentoDeveSer(string expectedStatus)
    {
        _scenarioContext.Response!.Status.Should().Be(expectedStatus);
    }

    [Then(@"o valor do pagamento deve ser (.*)")]
    public void ThenOValorDoPagamentoDeveSer(decimal expectedValue)
    {
        _scenarioContext.Response!.Value.Should().Be(expectedValue);
    }

    [Then(@"o pagamento deve estar persistido no banco de dados")]
    public async Task ThenOPagamentoDeveEstarPersistidoNoBancoDeDados()
    {
        var payment = await _paymentRepository.GetByIdAsync(_scenarioContext.Response!.Id);
        
        payment.Should().NotBeNull();
        payment!.OrderId.Should().Be(_scenarioContext.Request!.OrderId);
        payment.Value.Amount.Should().Be(_scenarioContext.Request.Value);
        payment.UserPaymentCode.Should().NotBeNullOrEmpty();
    }

    [Then(@"o pagamento existente deve ser retornado")]
    public void ThenOPagamentoExistenteDeveSerRetornado()
    {
        _scenarioContext.Exception.Should().BeNull();
        _scenarioContext.Response.Should().NotBeNull();
        _scenarioContext.WasCreated.Should().BeFalse();
    }

    [Then(@"o ID do pedido deve ser ""(.*)""")]
    public void ThenOIDDoPedidoDeveSer(string expectedOrderId)
    {
        _scenarioContext.Response!.OrderId.Should().Be(expectedOrderId);
    }
}

