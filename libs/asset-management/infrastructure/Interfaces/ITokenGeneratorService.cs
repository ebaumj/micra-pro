namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface ITokenCreatorService
{
    string GenerateUploadAccessToken(Guid assetId);
    string GenerateAccessToken(Guid assetId);
    string GenerateAccessToken();
}
