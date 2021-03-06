﻿using SuperiorHackBase.Core.ProcessInteraction.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperiorHackBase.Core.ProcessInteraction.Memory.Patterns
{
    public class Pattern
    {
        public byte[] Bytes { get; private set; }
        public string Mask { get; private set; }
        public Pointer ScanStart { get; private set; }
        public Pointer ScanEnd { get; private set; }
        public Pointer ScanLength { get { return ScanEnd - ScanStart; } }
        public string Module { get; private set; }
        public IPatternProcessor[] Processors { get; private set; }
        public bool FindMultiple { get; private set; }

        internal Pattern(Pointer from, Pointer to, byte[] bytes, string mask, IPatternProcessor[] processors, bool multiple)
        {
            ScanStart = from;
            ScanEnd = to;
            Bytes = bytes;
            Mask = mask;
            Processors = processors;
            FindMultiple = multiple;

            if (Bytes.Length != Mask.Length)
                throw new Exception("Bytes and Mask have to be of equal length");
        }

        internal Pattern(string module, byte[] bytes, string mask, IPatternProcessor[] processors, bool multiple)
        {
            Module = module;
            Bytes = bytes;
            Mask = mask;
            Processors = processors;
            FindMultiple = multiple;

            if (Bytes.Length != Mask.Length)
                throw new Exception("Bytes and Mask have to be of equal length");
        }

        public ScanResult[] Find(IHackContext context)
        {
            var results = new List<ScanResult>();

            var from = Pointer.Zero;
            var to = Pointer.Zero;
            IModule module = null;
            if (!string.IsNullOrEmpty(Module))
            {
                module = context.Process.FindModule(Module);
                if (module == null)
                    throw new Exception($"Module \"{Module}\" not found");

                from = module.BaseAddress;
                to = module.BaseAddress + module.Size;
            }
            else
            {
                from = ScanStart;
                to = ScanEnd;
                module = context.Process.Modules.FirstOrDefault(x => x.BaseAddress >= from && x.BaseAddress + x.Size <= to);
            }
            var length = to - from;

            var reader = new CachedStreamMemory(context.Memory, context.Process);
            reader.Position = (long)from.Address64;
            var buffer = new byte[4096];
            for (var i = 0; i < length; i += buffer.Length)
            {
                reader.Read(buffer, 0, buffer.Length);
                for (int b = 0; b < buffer.Length - Bytes.Length; b++)
                {
                    bool found = true;
                    for (int m = 0; m < Mask.Length; m++)
                    {
                        if (Mask[m] != '?' && Bytes[m] != buffer[b + m])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        var data = new byte[Bytes.Length];
                        Array.Copy(buffer, b, data, 0, data.Length);
                        var finding = new PatternFinding(data, from + b);
                        var result = finding.Process(context, Processors);
                        if (!FindMultiple)
                            return new ScanResult[] { result };

                        results.Add(result);

                    }
                }
            }
            return results.ToArray();
        }
    }
}
