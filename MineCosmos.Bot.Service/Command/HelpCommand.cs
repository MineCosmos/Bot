using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Kook;
using Kook.WebSocket;
using MineCosmos.Bot.Common;
using Newtonsoft.Json;

namespace MineCosmos.Bot.Service.Command;
public class HelpCommand : BaseCommand, ICommand
    {


        public HelpCommand(CommandData data)
        {
            _data = data;
        }

        public async Task<CommandExcuteResultModel> Execute()
        {
            List<IModuleBuilder> moduleBuilders = new List<IModuleBuilder>();

            foreach (BtnClickEventModel item in btnClickEventModels)
            {
                moduleBuilders.Add(
                    new SectionModuleBuilder
                    {
                        Text = new PlainTextElementBuilder { Content = item.Desc },
                        Mode = SectionAccessoryMode.Right,
                        Accessory = new ButtonElementBuilder
                        {
                            Theme = ButtonTheme.Primary,
                            Text = new PlainTextElementBuilder { Content = item.Text },
                            Click = ButtonClickEventType.ReturnValue,
                            Value = item.Value
                        }
                    }
                );
            }
            CardBuilder builder = new CardBuilder()
            { Modules = moduleBuilders, Theme = CardTheme.Secondary, Size = CardSize.Large };
            builder = builder.CardBuilderfoot();
            await _data.SocketMessage!.Channel.SendCardAsync(builder.Build());

            return Success();
        }

    }

