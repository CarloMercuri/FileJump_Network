using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class PacketBuffer : IDisposable
    {
        List<byte> _bufferList;
        byte[] _readBuffer;
        int _readPos;
        bool _buffUpdate = false;

        public PacketBuffer()
        {
            _bufferList = new List<byte>();
            _readPos = 0;
        }

        public int GetReadPos()
        {
            return _readPos;
        }

        public byte[] ToArray()
        {
            return _bufferList.ToArray();
        }

        public int Count()
        {
            return _bufferList.Count;
        }

        public int Lenght()
        {
            return Count() - _readPos;
        }

        public void Clear()
        {
            _bufferList.Clear();
            _readPos = 0;
        }

        // Write Data
        public void WriteBytes(byte[] input)
        {
            _bufferList.AddRange(input);
            _buffUpdate = true;
        }
        public void WriteByte(byte input)
        {
            _bufferList.Add(input);
            _buffUpdate = true;
        }
        public void WriteInteger(int input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }

        public void WriteUInt32(UInt32 input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }

        public void WriteULong(ulong input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }

        public void WriteLong(long input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }

        public void WriteUShort(ushort input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        public void WriteFloat(float input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        public void WriteString(string input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input.Length));
            _bufferList.AddRange(Encoding.ASCII.GetBytes(input));
            _buffUpdate = true;
        }

        public void WriteBool(bool input)
        {
            _bufferList.Add(Convert.ToByte(input));
            _buffUpdate = true;
        }

        // Read Data
        public int ReadInteger(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                int value = BitConverter.ToInt32(_readBuffer, _readPos);

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }

        public long ReadLong(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                long value = BitConverter.ToInt64(_readBuffer, _readPos);

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 8;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }

        public ulong ReadULong(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                ulong value = BitConverter.ToUInt64(_readBuffer, _readPos);

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 8;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }

        public ushort ReadUShort(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                ushort value = BitConverter.ToUInt16(_readBuffer, _readPos);

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }
        public float ReadFloat(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                float value = BitConverter.ToSingle(_readBuffer, _readPos);

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }
        public byte ReadByte(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                byte value = _readBuffer[_readPos];

                if (peek & _bufferList.Count > _readPos)
                {
                    _readPos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past it's limit!");
            }
        }
        public byte[] ReadBytes(int lenght, bool peek = true)
        {

            if (_buffUpdate)
            {
                _readBuffer = _bufferList.ToArray();
                _buffUpdate = false;
            }

            byte[] value = _bufferList.GetRange(_readPos, lenght).ToArray();

            if (peek & _bufferList.Count > _readPos)
            {
                _readPos += lenght;
            }
            return value;

        }
        public string ReadString(bool peek = true)
        {

            int lenght = ReadInteger(true);
            if (_buffUpdate)
            {
                _readBuffer = _bufferList.ToArray();
                _buffUpdate = false;
            }

            string value = Encoding.ASCII.GetString(_readBuffer, _readPos, lenght);

            if (peek & _bufferList.Count > _readPos)
            {
                _readPos += lenght;
            }
            return value;

        }

        // IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _bufferList.Clear();
                }
                _readPos = 0;
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
