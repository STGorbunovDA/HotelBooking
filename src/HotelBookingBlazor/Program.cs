using HotelBookingBlazor.Components;
using HotelBookingBlazor.Components.Account;
using HotelBookingBlazor.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBookingBlazor.Services;
using HotelBookingBlazor.Endpoints;
using HotelBookingBlazor.Services.Public;
using Stripe;
using HotelBookingBlazor.Models;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeApiKey");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddTransient<SeedService>()
                .AddTransient<IAmenitiesService, AmenitiesService>()
                .AddTransient<IRoomTypeService, RoomTypeService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IRoomsService, RoomsService>()
                .AddTransient<IBookingService, BookingService>()
                .AddTransient<IPaymentService, PaymentService>()
                .AddTransient<IEnquiryService, EnquiryService>()
                .AddTransient<ISubscriberService, SubscriberService>();

builder.Services.Configure<ContactInfoOptions>(builder.Configuration.GetSection(ContactInfoOptions.Key));

var app = builder.Build();

await InitializeAdminUser(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapCustomEndpoints();

app.Run();

static async Task InitializeAdminUser(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedDatabaseAsync();
}