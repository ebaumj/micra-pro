export interface CreateUpdateFileExecutorSchema {
  applicationPaths: {
    backend: string;
    frontend: string;
    asset_server: string;
  };
  outputPath: string;
  signingKey: string;
}
