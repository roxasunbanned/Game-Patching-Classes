using System;
using System.Diagnostics;
namespace game
{
	public class codecave
	{
        public bool Success
        { get; set; }
        public IntPtr TargetAddr
        { get; set; }
        public IntPtr AllocBaseAddr
        { get; set; }
        public byte[] OriginalCode
        { get; set; }
        public byte[] InjectedCode
        { get; set; }

        public bool doesExist()
		{
            if (!Success || TargetAddr == 0 || AllocBaseAddr == 0 || OriginalCode == null || InjectedCode.Length == null)
			{
				return false;
			}
			return true;
		}
    }
}