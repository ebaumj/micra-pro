import { throwInternalServerError } from '../../utils/errors';
import { getUserRepository } from '@micra-pro/recipe-hub/database';
import jwt from 'jsonwebtoken';
import bcrypt from 'bcrypt';
import emailValidator from 'node-email-verifier';
import nodemailer from 'nodemailer';

export const confirmIssuer = 'ConfirmNewUser';
const emailTokenLifetimeInMinutes = 30;

type CreateUserRequestBodyType = {
  username: string;
  email: string;
  password: string;
  clientId: string;
};

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  const body = JSON.parse(await readBody(event)) as CreateUserRequestBodyType;
  if (
    !(await emailValidator(body.email, {
      checkMx: true,
      checkDisposable: true,
      timeout: '5s',
    }))
  )
    throwInternalServerError();
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  const allUsers = await repository.getAll();
  if (
    allUsers.find((u) => u.email === body.email || u.username === body.username)
  )
    throwInternalServerError();
  if (body.username.includes('@')) throwInternalServerError();
  const newUser = {
    username: body.username,
    password: await bcrypt.hash(body.password, await bcrypt.genSalt()),
    clientId: body.clientId,
    email: body.email,
  };
  const userConfirmToken = jwt.sign(newUser, runtimeConfig.secrets.privateKey, {
    expiresIn: emailTokenLifetimeInMinutes * 60,
    issuer: confirmIssuer,
    audience: runtimeConfig.authorization.jwtAudience,
  });
  const url = `${runtimeConfig.userConfirmApi}?token=${userConfirmToken}`;
  const email = nodemailer.createTransport({
    service: 'gmail',
    auth: {
      user: runtimeConfig.secrets.emailServerAddress,
      pass: runtimeConfig.secrets.emailServerToken,
    },
  });
  const result = await email.sendMail({
    from: `"Micra Pro Recipe Hub" <${runtimeConfig.secrets.emailServerAddress}>`,
    to: body.email,
    subject: 'Confirm Micra Pro Recipe Hub Account',
    html: createHtml(url),
  });
  if (result.rejected.length > 1) throwInternalServerError();
  return { success: true };
});

const createHtml = (link: string) =>
  `<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional //EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
<head>
<!--[if gte mso 9]>
<xml>
  <o:OfficeDocumentSettings>
    <o:AllowPNG/>
    <o:PixelsPerInch>96</o:PixelsPerInch>
  </o:OfficeDocumentSettings>
</xml>
<![endif]-->
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="x-apple-disable-message-reformatting">
  <!--[if !mso]><!--><meta http-equiv="X-UA-Compatible" content="IE=edge"><!--<![endif]-->
  <title></title>
  
    <style type="text/css">
      
      @media only screen and (min-width: 620px) {
        .u-row {
          width: 600px !important;
        }

        .u-row .u-col {
          vertical-align: top;
        }

        
            .u-row .u-col-100 {
              width: 600px !important;
            }
          
      }

      @media only screen and (max-width: 620px) {
        .u-row-container {
          max-width: 100% !important;
          padding-left: 0px !important;
          padding-right: 0px !important;
        }

        .u-row {
          width: 100% !important;
        }

        .u-row .u-col {
          display: block !important;
          width: 100% !important;
          min-width: 320px !important;
          max-width: 100% !important;
        }

        .u-row .u-col > div {
          margin: 0 auto;
        }


}
    
body{margin:0;padding:0}table,td,tr{border-collapse:collapse;vertical-align:top}p{margin:0}.ie-container table,.mso-container table{table-layout:fixed}*{line-height:inherit}a[x-apple-data-detectors=true]{color:inherit!important;text-decoration:none!important}


table, td { color: #7e8f9e; } #u_body a { color: #000000; text-decoration: underline; }
    </style>
  
  

</head>

<body class="clean-body u_body" style="margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #0a243b;color: #7e8f9e">
  <!--[if IE]><div class="ie-container"><![endif]-->
  <!--[if mso]><div class="mso-container"><![endif]-->
  <table role="presentation" id="u_body" style="border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #0a243b;width:100%" cellpadding="0" cellspacing="0">
  <tbody>
  <tr style="vertical-align: top">
    <td style="word-break: break-word;border-collapse: collapse !important;vertical-align: top">
    <!--[if (mso)|(IE)]><table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td align="center" style="background-color: #0a243b;"><![endif]-->
    
  
  
<div class="u-row-container" style="padding: 0px;background-color: transparent">
  <div class="u-row" style="margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;">
    <div style="border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;">
      <!--[if (mso)|(IE)]><table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 0px;background-color: transparent;" align="center"><table role="presentation" cellpadding="0" cellspacing="0" border="0" style="width:600px;"><tr style="background-color: transparent;"><![endif]-->
      
<!--[if (mso)|(IE)]><td align="center" width="600" style="width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;" valign="top"><![endif]-->
<div class="u-col u-col-100" style="max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;">
  <div style="height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;">
  <!--[if (!mso)&(!IE)]><!--><div style="box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;"><!--<![endif]-->
  
<table style="font-family:helvetica,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
  <tbody>
    <tr>
      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:helvetica,sans-serif;" align="left">
        
  <!--[if mso]><table role="presentation" width="100%"><tr><td><![endif]-->
    <h1 style="margin: 0px; color: #ff3a61; line-height: 140%; text-align: left; word-wrap: break-word; font-size: 22px; font-weight: 400;"><span>Confirm Email</span></h1>
  <!--[if mso]></td></tr></table><![endif]-->

      </td>
    </tr>
  </tbody>
</table>

<table style="font-family:helvetica,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
  <tbody>
    <tr>
      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:helvetica,sans-serif;" align="left">
        
  <div style="font-size: 14px; color: #fefefe; line-height: 140%; text-align: left; word-wrap: break-word;">
    <p style="line-height: 140%;"><span style="color: rgb(253, 253, 253); line-height: 19.6px;"><a href="${link}" style="color: rgb(253, 253, 253);">${link}</a></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
  </div>
  


    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
    </td>
  </tr>
  </tbody>
  </table>
  <!--[if mso]></div><![endif]-->
  <!--[if IE]></div><![endif]-->
</body>

</html>
`.trim();
