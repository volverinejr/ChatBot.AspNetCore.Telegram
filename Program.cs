using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace lab001
{
    class Program
    {
        static ITelegramBotClient botClient;
        static String MsgSaudacao;
        static String MsgFinalizacao;

        static String[] OpcoesDoMenu;

        static String CaminhoBase = @"C:\dotnet\Telegram\lab001\atendimento\";

        static String MenuPrincipal = "";
        public const String Token = "987511593:AAGjCbh9rS5bgIUrHXNclOC3hLKsswky3F4";
        public const String Inicio = "9";


        static void Main(string[] args)
        {
            MsgSaudacao = System.IO.File.ReadAllText(CaminhoBase + "arq_saudacao.txt");
            MsgFinalizacao = System.IO.File.ReadAllText(CaminhoBase + "arq_finalizacao.txt");

            OpcoesDoMenu = System.IO.File.ReadAllLines(CaminhoBase + "arq_atendimento.txt");
            foreach (string line in OpcoesDoMenu)
            {
                MenuPrincipal += line + "\n";
            }

            botClient = new TelegramBotClient(Token);
            var me = botClient.GetMeAsync().Result;


            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Aperte qualquer tecla para sair");
            Console.ReadKey();


            botClient.StopReceiving();
        }



        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Recebendo a mensagem do chat {e.Message.Chat.Id}.");

                string arquivo = CaminhoBase + e.Message.Chat.Id + ".txt";


                if (!System.IO.File.Exists(arquivo))
                {
                    //Arquivo não existe

                    StreamWriter arq;
                    arq = System.IO.File.CreateText(arquivo);

                    foreach (string line in OpcoesDoMenu)
                    {
                        arq.WriteLine("#ESCOLHA_01#" + line);
                    }

                    arq.Close();

                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: ConverterParaEnviar(MsgSaudacao + "\n\n" + MenuPrincipal)
                    );

                }
                else
                {
                    //Arquivo já existe

                    IEnumerable<string> OpcoesDisponiveis =
                        System.IO.File.ReadLines(arquivo)
                          .Where(x => x.StartsWith("#ESCOLHA_01#"));

                    int OpcaoEscolhida = OpcoesDisponiveis
                      .Where(x => x.StartsWith("#ESCOLHA_01#" + e.Message.Text))
                      .Count();


                    String msgResposta = "";
                    if (OpcaoEscolhida == 0)
                    {
                        msgResposta = "Opção Inválida: \n\n";
                        foreach (string line in OpcoesDisponiveis)
                        {
                            msgResposta += line.Replace("#ESCOLHA_01#", "") + "\n";
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: ConverterParaEnviar(msgResposta)
                        );
                    }
                    else if (e.Message.Text == Inicio)
                    {
                        if (System.IO.File.Exists(arquivo))
                        {
                            System.IO.File.Delete(arquivo);

                            await botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: ConverterParaEnviar(MsgFinalizacao)
                            );
                        }
                    }
                    else if (OpcaoEscolhida > 0 && !System.IO.File.Exists(CaminhoBase + e.Message.Text + ".txt"))
                    {
                        msgResposta = "Opção em Manutenção no momento: \n\n";
                        foreach (string line in OpcoesDisponiveis)
                        {
                            msgResposta += line.Replace("#ESCOLHA_01#", "") + "\n";
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: ConverterParaEnviar(msgResposta)
                        );
                    }
                    else
                    {
                        StreamWriter arq;
                        arq = System.IO.File.CreateText(arquivo);

                        String[] Opcoes = System.IO.File.ReadAllLines(CaminhoBase + e.Message.Text + ".txt");
                        foreach (string line in Opcoes)
                        {
                            arq.WriteLine("#ESCOLHA_01#" + line);

                            msgResposta += line + "\n";
                        }
                        arq.Close();

                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: ConverterParaEnviar(msgResposta)
                        );

                    }

                }

            }
        }

        public static String ConverterParaEnviar(String msg)
        {
            msg = msg.Replace(":last_track_button:", char.ConvertFromUtf32(0X23EE));
            msg = msg.Replace(":credit_card:", char.ConvertFromUtf32(0X1F4B3));
            msg = msg.Replace(":toolbox:", char.ConvertFromUtf32(0X1F9F0));

            msg = msg.Replace(":wave:", char.ConvertFromUtf32(0X1F44B));

            msg = msg.Replace(":crossed_fingers:", char.ConvertFromUtf32(0X1F91E));

            msg = msg.Replace(":house:", char.ConvertFromUtf32(0X1F3E0));
            msg = msg.Replace(":moneybag:", char.ConvertFromUtf32(0X1F4B0));
            msg = msg.Replace(":heavy_dollar_sign:", char.ConvertFromUtf32(0X1F4B2));

            msg = msg.Replace(":thumbs_up:", char.ConvertFromUtf32(0X1F44D));
            msg = msg.Replace(":clapping_hands:", char.ConvertFromUtf32(0X1F44F));
            msg = msg.Replace(":wrapped_gift:", char.ConvertFromUtf32(0X1F381));

            msg = msg.Replace(":chequered_flag:", char.ConvertFromUtf32(0X1F3C1));
            

            return msg;
        }



    }
}
