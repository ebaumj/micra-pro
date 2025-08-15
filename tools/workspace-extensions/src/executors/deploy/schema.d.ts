export interface DeployExecutorSchema {
  sshTarget: { target: string; user: string };
  affectedServices?: string[];
  paths: { origin: string; destination: string };
}
