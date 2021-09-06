using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Dependency
{
    //そもそもなぜAndroid側に各国語翻訳を置いたのかと言う…
    public interface IPlatformLocalization
    {
        string WordLeft { get; }
        string WordRight { get; }
    }
}
