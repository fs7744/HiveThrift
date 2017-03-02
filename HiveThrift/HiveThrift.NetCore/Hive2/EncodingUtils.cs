﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thrift.Transport
{
    public static class EncodingUtils
    {
        public static void encodeBigEndian(int integer, byte[] buf)
        {
            encodeBigEndian(integer, buf, 0);
        }

        public static void encodeBigEndian(int integer, byte[] buf, int offset)
        {
            buf[offset] = (byte)(0xff & (integer >> 24));
            buf[offset + 1] = (byte)(0xff & (integer >> 16));
            buf[offset + 2] = (byte)(0xff & (integer >> 8));
            buf[offset + 3] = (byte)(0xff & (integer));
        }

        public static int decodeBigEndian(byte[] buf)
        {
            return decodeBigEndian(buf, 0);
        }

        public static int decodeBigEndian(byte[] buf, int offset)
        {
            return ((buf[offset] & 0xff) << 24) | ((buf[offset + 1] & 0xff) << 16)
                | ((buf[offset + 2] & 0xff) << 8) | ((buf[offset + 3] & 0xff));
        }

        public static void encodeFrameSize(int frameSize, byte[] buf)
        {
            buf[0] = (byte)(0xff & (frameSize >> 24));
            buf[1] = (byte)(0xff & (frameSize >> 16));
            buf[2] = (byte)(0xff & (frameSize >> 8));
            buf[3] = (byte)(0xff & (frameSize));
        }

        public static int decodeFrameSize(byte[] buf)
        {
            return
              ((buf[0] & 0xff) << 24) |
              ((buf[1] & 0xff) << 16) |
              ((buf[2] & 0xff) << 8) |
              ((buf[3] & 0xff));
        }
    }
}

namespace Hive2
{
    public static class Utils
    {
        public static bool IsEmpty(this IEnumerable enumerable)
        {
            var enumerator = enumerable != null ? enumerable.GetEnumerator() : null;
            return enumerator == null || !enumerator.MoveNext();
        }

        public static IEnumerable<List<T>> SplitByCount<T>(this IEnumerable<T> list, int count)
        {
            if (list == null || count == 0) throw new ArgumentNullException("list can't be null or count can't be 0");
            int sendCount = 0;
            List<T> result = null;
            while (true)
            {
                result = list.Skip(sendCount).Take(count).ToList();
                if (result.IsEmpty())
                    break;
                sendCount += count;
                yield return result;
            }
        }
    }
}