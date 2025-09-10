using Azure.AI.OpenAI;
using DWG.MCP.Client.Web.Components;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureOpenAIClient("openai");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddChatClient(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var aiClient = sp.GetRequiredService<AzureOpenAIClient>();
    return aiClient.GetChatClient(config.GetValue<string>("openAiModel")).AsIChatClient();
})
.UseFunctionInvocation()
.UseLogging();

builder.Services.AddSingleton<IMcpClient?>(sp =>
{
    // TODO change based on other example to implement a custom McpClient
    return null; // Use null-forgiving operator since we know we want to allow null
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
