import fs from 'fs';
import path from 'path';
import os from 'os';
import useAppconfig from './appconfig';
import AdmZip from 'adm-zip';
import SftpClient from 'ssh2-sftp-client';

export const backupAssets = async (
  backupServer: string,
  directory: string,
  username: string,
  password: string,
) => {
  console.log('Starting Asset backup...');
  const appConfig = useAppconfig();
  const sourceAssetDirectory = path.resolve(
    process.cwd(),
    appConfig.blobStorage.folder,
  );
  const fileName = `assets.zip`;
  const filePath = path.join(os.tmpdir(), fileName);
  const zip = new AdmZip();
  zip.addLocalFolder(sourceAssetDirectory, '');
  await zip.writeZipPromise(filePath);
  const sftp = new SftpClient();
  try {
    await sftp.connect({
      host: backupServer,
      port: 22,
      username: username,
      password: password,
    });
    const remoteFilePath = path.posix.join(directory, fileName);
    await sftp.put(filePath, remoteFilePath);
  } catch (err) {
    console.error('SFTP upload failed:', err);
    throw err;
  } finally {
    await sftp.end();
  }
  if (fs.existsSync(filePath)) fs.unlinkSync(filePath);
  console.log('Asset backup completed successfully!');
};
