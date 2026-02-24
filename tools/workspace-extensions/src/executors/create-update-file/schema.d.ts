export interface CreateUpdateFileExecutorSchema {
  applicationPaths: {
    backend: string;
    frontend: string;
    asset_server: string;
  };
  appSettingsFile: string;
  outputPath: string;
  signingKey: string;
}
