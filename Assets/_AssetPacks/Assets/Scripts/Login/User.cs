using System;
using JetBrains.Annotations;

[Serializable]
public class User
{
    [NotNull]
    public string Nickname
    {
        get => Nickname;
        set => Nickname = value ?? throw new ArgumentNullException(nameof(value));
    }
}