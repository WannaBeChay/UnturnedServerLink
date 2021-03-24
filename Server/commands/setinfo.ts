import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {USLClient} from '../models/USLClient';

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (!(await client.checkAdmin(msg))) return;

  var db: Database = await new Database('settings').safe();

  msg.delete({timeout: 100});
  msg.channel.send(client.embed('Creating Server Info Panel')).then((sent) => {
    db.setJSON('serverInfo', {channel: sent.channel.id, message: sent.id});
  });
};
