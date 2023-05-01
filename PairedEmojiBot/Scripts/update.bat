docker build C:\Apps\PairedEmojiBot\ -t paired-emoji-bot-image
docker stop paired-emoji-bot
docker rm paired-emoji-bot
docker run -d -it --name paired-emoji-bot --mount type=bind,source=c:\Apps\PairedEmojiBot\Db,target=c:\app\DbFiles paired-emoji-bot-image