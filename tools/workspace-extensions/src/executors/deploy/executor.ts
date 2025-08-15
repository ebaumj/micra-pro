import { DeployExecutorSchema } from './schema';
import { execSync } from 'child_process';

export default async function runExecutor(options: DeployExecutorSchema) {
  console.log(options.paths.origin);
  if (options.affectedServices?.length ?? 0 > 0) {
    console.log(
      `Shutting down services: ${options.affectedServices.join(', ')}`,
    );
    execSync(
      `ssh ${options.sshTarget.target} sudo systemctl stop ${options.affectedServices.join(' ')}`,
    );
  }
  console.log(
    `Copying files from ${options.paths.origin} to ${options.paths.destination}.`,
  );
  execSync(
    `ssh ${options.sshTarget.target} sudo chmod +r ${options.paths.destination}`,
  );
  execSync(
    `scp -r ${options.paths.origin} ${options.sshTarget.user}@${options.sshTarget.target}:${options.paths.destination}`,
  );
  if (options.affectedServices?.length ?? 0 > 0) {
    console.log(`Starting services: ${options.affectedServices.join(', ')}`);
    execSync(
      `ssh ${options.sshTarget.target} sudo systemctl start ${options.affectedServices.join(' ')}`,
    );
  }
  return { success: true };
}
