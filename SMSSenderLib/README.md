# SMSSenderLib

A flexible .NET library for sending SMS messages through multiple providers, following the same architecture as EmailSenderLib.

## Supported Providers

### Global / Enterprise-Grade Providers
- **Twilio** - Industry-leading platform with excellent C# SDK and documentation
- **Vonage (Nexmo)** - Solid provider with good European coverage
- **MessageBird** - Strong presence in Europe with comprehensive API
- **Sinch** - Excellent Nordic presence and reliability
- **Gatewayapi** - Reliable SMS gateway service

### Africa / Kenya-Focused Providers
- **Africa's Talking** - Leading African SMS provider
- **IntaSend** - Kenyan payment and SMS platform
- **Safaricom** - M-Pesa ecosystem SMS integration

### Local Options
- **Local Modem** - Send SMS via GSM modem connected to serial port

## Installation

Add the SMSSenderLib project reference to your solution:

```bash
dotnet add reference ../SMSSenderLib/SMSSenderLib.csproj
```

## Configuration

Add SMS settings to your `appsettings.json`:

### Twilio Example
```json
{
  "SmsSettings": {
    "Provider": "Twilio",
    "Twilio": {
      "AccountSid": "your-account-sid",
      "AuthToken": "your-auth-token",
      "FromNumber": "+1234567890",
      "MessagingServiceSid": "optional-messaging-service-sid"
    }
  }
}
```

### Vonage (Nexmo) Example
```json
{
  "SmsSettings": {
    "Provider": "Vonage",
    "Vonage": {
      "ApiKey": "your-api-key",
      "ApiSecret": "your-api-secret",
      "FromNumber": "+1234567890",
      "Brand": "YourBrand"
    }
  }
}
```

### MessageBird Example
```json
{
  "SmsSettings": {
    "Provider": "MessageBird",
    "MessageBird": {
      "ApiKey": "your-api-key",
      "Originator": "YourBrand",
      "Reference": "optional-reference"
    }
  }
}
```

### Sinch Example
```json
{
  "SmsSettings": {
    "Provider": "Sinch",
    "Sinch": {
      "ServicePlanId": "your-service-plan-id",
      "ApiToken": "your-api-token",
      "FromNumber": "+1234567890",
      "Region": "us"
    }
  }
}
```

### Gatewayapi Example
```json
{
  "SmsSettings": {
    "Provider": "Gatewayapi",
    "Gatewayapi": {
      "ApiToken": "your-api-token",
      "Sender": "YourBrand"
    }
  }
}
```

### Africa's Talking Example
```json
{
  "SmsSettings": {
    "Provider": "AfricasTalking",
    "AfricasTalking": {
      "Username": "your-username",
      "ApiKey": "your-api-key",
      "From": "YourBrand",
      "Environment": "production"
    }
  }
}
```

### IntaSend Example
```json
{
  "SmsSettings": {
    "Provider": "IntaSend",
    "IntaSend": {
      "ApiKey": "your-api-key",
      "PublicKey": "your-public-key",
      "SenderId": "YourBrand",
      "TestMode": false
    }
  }
}
```

### Safaricom Example
```json
{
  "SmsSettings": {
    "Provider": "Safaricom",
    "Safaricom": {
      "ConsumerKey": "your-consumer-key",
      "ConsumerSecret": "your-consumer-secret",
      "ShortCode": "your-shortcode",
      "InitiatorName": "your-initiator",
      "SecurityCredential": "your-credential",
      "Environment": "sandbox",
      "CallbackUrl": "https://your-callback-url.com"
    }
  }
}
```

### Local Modem Example
```json
{
  "SmsSettings": {
    "Provider": "LocalModem",
    "LocalModem": {
      "ComPort": "COM3",
      "BaudRate": 115200,
      "DataBits": 8,
      "Parity": "None",
      "StopBits": "One",
      "ReadTimeout": 3000,
      "WriteTimeout": 3000,
      "Pin": "1234"
    }
  }
}
```

## Usage

### 1. Register Services in Program.cs

```csharp
using SMSSenderLib.Infrastructure.Settings;
using SMSSenderLib.Factories;
using SMSSenderLib.Interfaces;

// Configure settings
builder.Services.Configure<SmsSettings>(
    builder.Configuration.GetSection("SmsSettings"));

// Register factory and sender
builder.Services.AddSingleton<SmsSenderFactory>(sp =>
{
    var smsSettings = sp.GetRequiredService<IOptions<SmsSettings>>().Value;
    return new SmsSenderFactory(smsSettings);
});

builder.Services.AddSingleton<ISmsSender>(sp =>
{
    var factory = sp.GetRequiredService<SmsSenderFactory>();
    return factory.CreateSmsSender();
});
```

### 2. Inject and Use in Your Services

```csharp
using SMSSenderLib.Interfaces;

public class NotificationService
{
    private readonly ISmsSender _smsSender;

    public NotificationService(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }

    public async Task SendVerificationCode(string phoneNumber, string code)
    {
        var message = $"Your verification code is: {code}";
        await _smsSender.SendSmsAsync(phoneNumber, message);
    }

    public async Task SendBulkNotification(List<string> phoneNumbers, string message)
    {
        await _smsSender.SendSmsAsync(phoneNumbers, message);
    }
}
```

## Implementation Notes

Each provider implementation includes:
- Placeholder code with commented examples
- Notes about required NuGet packages
- API endpoint references where applicable

To fully implement a provider, you'll need to:
1. Install the provider's SDK or use HttpClient for REST APIs
2. Uncomment and complete the implementation code
3. Add proper error handling and logging
4. Test with the provider's sandbox/test environment

## Provider-Specific Resources

- **Twilio**: https://www.twilio.com/docs/sms
- **Vonage**: https://developer.vonage.com/messaging/sms/overview
- **MessageBird**: https://developers.messagebird.com/api/sms-messaging/
- **Sinch**: https://developers.sinch.com/docs/sms/
- **Gatewayapi**: https://gatewayapi.com/docs/
- **Africa's Talking**: https://developers.africastalking.com/docs/sms
- **IntaSend**: https://developers.intasend.com/
- **Safaricom**: https://developer.safaricom.co.ke/

## License

Same as the parent project.
