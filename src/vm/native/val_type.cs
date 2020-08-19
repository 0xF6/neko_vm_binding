namespace Neko
{
    public enum val_type : uint
    {
        VAL_INT			= 0xFF,
        VAL_NULL		= 0,
        VAL_FLOAT		= 1,
        VAL_BOOL		= 2,
        VAL_STRING		= 3,
        VAL_OBJECT		= 4,
        VAL_ARRAY		= 5,
        VAL_FUNCTION	= 6,
        VAL_ABSTRACT	= 7,
        VAL_INT32		= 8,
        VAL_PRIMITIVE	= 6 | 16,
        VAL_JITFUN		= 6 | 32,
        VAL_32_BITS		= 0xFFFFFFFF
    }
}