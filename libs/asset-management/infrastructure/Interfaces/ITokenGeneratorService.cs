namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface ITokenCreatorService
{
    string GenerateUploadAccessToken(Guid assetId);
    string GenerateAccessToken(Guid assetId);
    string GenerateWebhookAccessToken(string subject);
    string GenerateRefreshToken();
    string GenerateAccessToken();
}
