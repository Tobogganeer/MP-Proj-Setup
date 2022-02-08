using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;

public struct AudioMessage : INetworkMessage
{
    public AudioArray sound;
    public byte audioIndex;
    public Vector3 position;
    public NetworkID parent;
    public float maxDistance;
    public AudioCategory category;
    public float volume;
    public float minPitch;
    public float maxPitch;

    public AudioMessageFlags flags;

    public AudioMessage(AudioArray sound, byte audioIndex, Vector3 position, NetworkID parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = AudioManager.DEFAULT_MIN_PITCH, float maxPitch = AudioManager.DEFAULT_MAX_PITCH)
    {
        this.sound = sound;
        this.audioIndex = audioIndex;
        this.position = position;
        this.parent = parent;
        this.maxDistance = maxDistance;
        this.category = category;
        this.volume = volume;
        this.minPitch = minPitch;
        this.maxPitch = maxPitch;

        flags = AudioMessageFlags.None;
    }

    public AudioMessage IsGlobal()
    {
        flags |= AudioMessageFlags.Global;
        return this;
    }

    public AudioMessage UseParent()
    {
        flags |= AudioMessageFlags.Parent;
        return this;
    }

    public AudioMessage UseDistance()
    {
        flags |= AudioMessageFlags.Distance;
        return this;
    }

    public AudioMessage UseVolume()
    {
        flags |= AudioMessageFlags.Volume;
        return this;
    }

    public AudioMessage UsePitch()
    {
        flags |= AudioMessageFlags.Pitch;
        return this;
    }

    public AudioMessage UseCategory()
    {
        flags |= AudioMessageFlags.Category;
        return this;
    }

    public void AddToMessage(Message message)
    {
        message.Add((byte)flags);
        message.Add((ushort)sound);
        message.Add(audioIndex);

        if (!flags.HasFlag(AudioMessageFlags.Global))
        {
            message.Add(position);

            if (flags.HasFlag(AudioMessageFlags.Parent) && parent != null)
                message.Add(parent);

            if (flags.HasFlag(AudioMessageFlags.Distance))
                message.Add(maxDistance);
        }

        if (flags.HasFlag(AudioMessageFlags.Volume))
            message.Add(volume);

        if (flags.HasFlag(AudioMessageFlags.Pitch))
        {
            message.Add(minPitch);
            message.Add(maxPitch);
        }

        if (flags.HasFlag(AudioMessageFlags.Category))
            message.Add((byte)category);
    }

    public void Deserialize(Message message)
    {
        flags = (AudioMessageFlags)message.GetByte();
        sound = (AudioArray)message.GetUShort();
        audioIndex = message.GetByte();

        if (!flags.HasFlag(AudioMessageFlags.Global))
        {
            position = message.GetVector3();

            if (flags.HasFlag(AudioMessageFlags.Parent))
                parent = message.GetNetworkID();

            if (flags.HasFlag(AudioMessageFlags.Distance))
                maxDistance = message.GetFloat();
            else
                maxDistance = 10f;
        }

        if (flags.HasFlag(AudioMessageFlags.Volume))
            volume = message.GetFloat();
        else
            volume = 1f;

        if (flags.HasFlag(AudioMessageFlags.Pitch))
        {
            minPitch = message.GetFloat();
            maxPitch = message.GetFloat();
        }
        else
        {
            minPitch = AudioManager.DEFAULT_MIN_PITCH;
            maxPitch = AudioManager.DEFAULT_MAX_PITCH;
        }

        if (flags.HasFlag(AudioMessageFlags.Category))
            category = (AudioCategory)message.GetByte();
        else
            category = AudioCategory.SFX;
    }

    [System.Flags]
    public enum AudioMessageFlags : byte
    {
        None        = 0,
        Global      = 1 << 0,
        Parent      = 1 << 1,
        Distance    = 1 << 2,
        Volume      = 1 << 3,
        Pitch       = 1 << 4,
        Category    = 1 << 5,
    }
}

public struct PlayerAnimationMessage : INetworkMessage
{
    public bool crouching;
    public bool grounded;
    public float x;
    public float y;
    public Vector3 lookDirection;

    public PlayerAnimationMessage(bool crouching, bool grounded, float x, float y, Vector3 lookDirection)
    {
        this.crouching = crouching;
        this.grounded = grounded;
        this.x = x;
        this.y = y;
        this.lookDirection = lookDirection;
    }

    public void AddToMessage(Message message)
    {
        int i_crouch = crouching ? 1 : 0;
        int i_ground = grounded ? 1 : 0;
        byte flags = (byte)(i_crouch << 1 | i_ground);

        message.Add(flags);

        message.Add(Compression.Vector.Quantize_8bit(x, -1, 1, 8));
        message.Add(Compression.Vector.Quantize_8bit(y, -1, 1, 8));
        //message.Add(Quaternion.Euler(lookDirection));
        // Quaternions compressed to a single uint
        // DONT WORK

        message.Add(lookDirection);
    }

    public void Deserialize(Message message)
    {
        byte flags = message.GetByte();

        crouching = (flags & 1 << 1) != 0;
        grounded = (flags & 1 << 0) != 0;

        x = Compression.Vector.Dequantize(message.GetByte(), -1, 1, 8);
        y = Compression.Vector.Dequantize(message.GetByte(), -1, 1, 8);
        //lookDirection = message.GetQuaternion().eulerAngles;
        lookDirection = message.GetVector3();
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerAnimationMessage message &&
               crouching == message.crouching &&
               grounded == message.grounded &&
               x == message.x &&
               y == message.y &&
               lookDirection.Equals(message.lookDirection);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(PlayerAnimationMessage ls, PlayerAnimationMessage rs)
    {
        return ls.Equals(rs);
    }

    public static bool operator !=(PlayerAnimationMessage ls, PlayerAnimationMessage rs)
    {
        return !ls.Equals(rs);
    }
}
