using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using DXGI;
using GlobalStructures;

namespace MFMediaEngine
{
    [ComImport]
    [Guid("fee7c112-e776-42b5-9bbf-0048524e2bd5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineNotify
    {
        //[return: MarshalAs(UnmanagedType.U4)]
        [PreserveSig]
        HRESULT EventNotify(MF_MEDIA_ENGINE_EVENT nEvent, IntPtr param1, uint param2);
    }

    public enum MF_MEDIA_ENGINE_EVENT
    {
        MF_MEDIA_ENGINE_EVENT_LOADSTART = 1,
        MF_MEDIA_ENGINE_EVENT_PROGRESS = 2,
        MF_MEDIA_ENGINE_EVENT_SUSPEND = 3,
        MF_MEDIA_ENGINE_EVENT_ABORT = 4,
        MF_MEDIA_ENGINE_EVENT_ERROR = 5,
        MF_MEDIA_ENGINE_EVENT_EMPTIED = 6,
        MF_MEDIA_ENGINE_EVENT_STALLED = 7,
        MF_MEDIA_ENGINE_EVENT_PLAY = 8,
        MF_MEDIA_ENGINE_EVENT_PAUSE = 9,
        MF_MEDIA_ENGINE_EVENT_LOADEDMETADATA = 10,
        MF_MEDIA_ENGINE_EVENT_LOADEDDATA = 11,
        MF_MEDIA_ENGINE_EVENT_WAITING = 12,
        MF_MEDIA_ENGINE_EVENT_PLAYING = 13,
        MF_MEDIA_ENGINE_EVENT_CANPLAY = 14,
        MF_MEDIA_ENGINE_EVENT_CANPLAYTHROUGH = 15,
        MF_MEDIA_ENGINE_EVENT_SEEKING = 16,
        MF_MEDIA_ENGINE_EVENT_SEEKED = 17,
        MF_MEDIA_ENGINE_EVENT_TIMEUPDATE = 18,
        MF_MEDIA_ENGINE_EVENT_ENDED = 19,
        MF_MEDIA_ENGINE_EVENT_RATECHANGE = 20,
        MF_MEDIA_ENGINE_EVENT_DURATIONCHANGE = 21,
        MF_MEDIA_ENGINE_EVENT_VOLUMECHANGE = 22,
        MF_MEDIA_ENGINE_EVENT_FORMATCHANGE = 1000,
        MF_MEDIA_ENGINE_EVENT_PURGEQUEUEDEVENTS = 1001,
        MF_MEDIA_ENGINE_EVENT_TIMELINE_MARKER = 1002,
        MF_MEDIA_ENGINE_EVENT_BALANCECHANGE = 1003,
        MF_MEDIA_ENGINE_EVENT_DOWNLOADCOMPLETE = 1004,
        MF_MEDIA_ENGINE_EVENT_BUFFERINGSTARTED = 1005,
        MF_MEDIA_ENGINE_EVENT_BUFFERINGENDED = 1006,
        MF_MEDIA_ENGINE_EVENT_FRAMESTEPCOMPLETED = 1007,
        MF_MEDIA_ENGINE_EVENT_NOTIFYSTABLESTATE = 1008,
        MF_MEDIA_ENGINE_EVENT_FIRSTFRAMEREADY = 1009,
        MF_MEDIA_ENGINE_EVENT_TRACKSCHANGE = 1010,
        MF_MEDIA_ENGINE_EVENT_OPMINFO = 1011,
        MF_MEDIA_ENGINE_EVENT_RESOURCELOST = 1012,
        MF_MEDIA_ENGINE_EVENT_DELAYLOADEVENT_CHANGED = 1013,
        MF_MEDIA_ENGINE_EVENT_STREAMRENDERINGERROR = 1014,
        MF_MEDIA_ENGINE_EVENT_SUPPORTEDRATES_CHANGED = 1015,
        MF_MEDIA_ENGINE_EVENT_AUDIOENDPOINTCHANGE = 1016
    }

    [ComImport]
    [Guid("4D645ACE-26AA-4688-9BE1-DF3516990B93")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineClassFactory
    {
        HRESULT CreateInstance(uint dwFlags, IMFAttributes pAttr, out IMFMediaEngine ppPlayer);
        HRESULT CreateTimeRange(out IMFMediaTimeRange ppTimeRange);
        HRESULT CreateError(out IMFMediaError ppError);
    }

    [ComImport]
    [Guid("2cd2d921-c447-44a7-a13c-4adabfc247e3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAttributes
    {
        HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        HRESULT DeleteItem(ref Guid guidKey);
        HRESULT DeleteAllItems();
        HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        HRESULT SetDouble(ref Guid guidKey, double fValue);
        HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        HRESULT SetString(ref Guid guidKey, string wszValue);
        HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        HRESULT LockStore();
        HRESULT UnlockStore();
        HRESULT GetCount(out uint pcItems);
        HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        HRESULT CopyAllItems(IMFAttributes pDest = null);
    }

    public enum MF_ATTRIBUTES_MATCH_TYPE
    {
        MF_ATTRIBUTES_MATCH_OUR_ITEMS = 0,
        MF_ATTRIBUTES_MATCH_THEIR_ITEMS = 1,
        MF_ATTRIBUTES_MATCH_ALL_ITEMS = 2,
        MF_ATTRIBUTES_MATCH_INTERSECTION = 3,
        MF_ATTRIBUTES_MATCH_SMALLER = 4
    }

    public enum MF_ATTRIBUTE_TYPE
    {
        MF_ATTRIBUTE_UINT32 = VARENUM.VT_UI4,
        MF_ATTRIBUTE_UINT64 = VARENUM.VT_UI8,
        MF_ATTRIBUTE_DOUBLE = VARENUM.VT_R8,
        MF_ATTRIBUTE_GUID = VARENUM.VT_CLSID,
        MF_ATTRIBUTE_STRING = VARENUM.VT_LPWSTR,
        MF_ATTRIBUTE_BLOB = (VARENUM.VT_VECTOR | VARENUM.VT_UI1),
        MF_ATTRIBUTE_IUNKNOWN = VARENUM.VT_UNKNOWN
    }

    [ComImport]
    [Guid("98a1b0bb-03eb-4935-ae7c-93c1fa0e1c93")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngine
    {
        HRESULT GetError(out IMFMediaError ppError);
        HRESULT SetErrorCode(MF_MEDIA_ENGINE_ERR error);
        HRESULT SetSourceElements(IMFMediaEngineSrcElements pSrcElements);
        [PreserveSig]
        HRESULT SetSource(string pUrl);
        HRESULT GetCurrentSource(out string ppUrl);
        [PreserveSig]
        byte GetNetworkState();
        [PreserveSig]
        MF_MEDIA_ENGINE_PRELOAD GetPreload();
        HRESULT SetPreload(MF_MEDIA_ENGINE_PRELOAD Preload);
        HRESULT GetBuffered(out IMFMediaTimeRange ppBuffered);
        [PreserveSig]
        HRESULT Load();
        HRESULT CanPlayType(string type, out MF_MEDIA_ENGINE_CANPLAY pAnswer);
        [PreserveSig]
        MF_MEDIA_ENGINE_READY GetReadyState();
        [PreserveSig]
        bool IsSeeking();
        [PreserveSig]
        double GetCurrentTime();
        HRESULT SetCurrentTime(double seekTime);
        [PreserveSig]
        double GetStartTime();
        [PreserveSig]
        double GetDuration();
        [PreserveSig]
        bool IsPaused();
        [PreserveSig]
        double GetDefaultPlaybackRate();
        HRESULT SetDefaultPlaybackRate(double Rate);
        [PreserveSig]
        double GetPlaybackRate();
        HRESULT SetPlaybackRate(double Rate);
        HRESULT GetPlayed(out IMFMediaTimeRange ppPlayed);
        HRESULT GetSeekable(out IMFMediaTimeRange ppSeekable);
        [PreserveSig]
        bool IsEnded();
        [PreserveSig]
        bool GetAutoPlay();
        HRESULT SetAutoPlay(bool AutoPlay);
        [PreserveSig]
        bool GetLoop();
        HRESULT SetLoop(bool Loop);
        HRESULT Play();
        HRESULT Pause();
        [PreserveSig]
        bool GetMuted();
        HRESULT SetMuted(bool Muted);
        [PreserveSig]
        double GetVolume();
        HRESULT SetVolume(double Volume);
        [PreserveSig]
        bool HasVideo();
        [PreserveSig]
        bool HasAudio();
        HRESULT GetNativeVideoSize(out uint cx, out uint cy);
        HRESULT GetVideoAspectRatio(out uint cx, out uint cy);
        HRESULT Shutdown();
        [PreserveSig]
        HRESULT TransferVideoFrame(IntPtr pDstSurf, ref MFVideoNormalizedRect pSrc, ref RECT pDst, ref MFARGB pBorderClr);
        [PreserveSig]
        HRESULT OnVideoStreamTick(out long pPts);
    }

    public enum MF_MEDIA_ENGINE_READY : byte
    {
        MF_MEDIA_ENGINE_READY_HAVE_NOTHING = 0,
        MF_MEDIA_ENGINE_READY_HAVE_METADATA = 1,
        MF_MEDIA_ENGINE_READY_HAVE_CURRENT_DATA = 2,
        MF_MEDIA_ENGINE_READY_HAVE_FUTURE_DATA = 3,
        MF_MEDIA_ENGINE_READY_HAVE_ENOUGH_DATA = 4
    };

    [ComImport]
    [Guid("fc0e10d2-ab2a-4501-a951-06bb1075184c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaError
    {
        [PreserveSig]
        byte GetErrorCode();
        HRESULT GetExtendedErrorCode();
        HRESULT SetErrorCode(MF_MEDIA_ENGINE_ERR error);
        HRESULT SetExtendedErrorCode(HRESULT error);
    }

    public enum MF_MEDIA_ENGINE_ERR
    {
        MF_MEDIA_ENGINE_ERR_NOERROR = 0,
        MF_MEDIA_ENGINE_ERR_ABORTED = 1,
        MF_MEDIA_ENGINE_ERR_NETWORK = 2,
        MF_MEDIA_ENGINE_ERR_DECODE = 3,
        MF_MEDIA_ENGINE_ERR_SRC_NOT_SUPPORTED = 4,
        MF_MEDIA_ENGINE_ERR_ENCRYPTED = 5
    }

    [ComImport]
    [Guid("7a5e5354-b114-4c72-b991-3131d75032ea")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineSrcElements
    {
        [PreserveSig]
        uint GetLength();
        [PreserveSig]
        HRESULT GetURL(uint index, out string pURL);
        [PreserveSig]
        HRESULT GetType(uint index, out string pType);
        [PreserveSig]
        HRESULT GetMedia(uint index, out string pMedia);
        [PreserveSig]
        HRESULT AddElement(string pURL, string pType, string pMedia);
        [PreserveSig]
        HRESULT RemoveAllElements();
    }

    public enum MF_MEDIA_ENGINE_PRELOAD
    {
        MF_MEDIA_ENGINE_PRELOAD_MISSING = 0,
        MF_MEDIA_ENGINE_PRELOAD_EMPTY = 1,
        MF_MEDIA_ENGINE_PRELOAD_NONE = 2,
        MF_MEDIA_ENGINE_PRELOAD_METADATA = 3,
        MF_MEDIA_ENGINE_PRELOAD_AUTOMATIC = 4
    }

    public enum MF_MEDIA_ENGINE_CANPLAY
    {
        MF_MEDIA_ENGINE_CANPLAY_NOT_SUPPORTED = 0,
        MF_MEDIA_ENGINE_CANPLAY_MAYBE = 1,
        MF_MEDIA_ENGINE_CANPLAY_PROBABLY = 2
    }

    [ComImport]
    [Guid("db71a2fc-078a-414e-9df9-8c2531b0aa6c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaTimeRange
    {
        [PreserveSig]
        uint GetLength();
        HRESULT GetStart(uint index, out double pStart);
        HRESULT GetEnd(uint index, out double pEnd);
        [PreserveSig]
        bool ContainsTime(double time);
        HRESULT AddRange(double startTime, double endTime);
        HRESULT Clear();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFVideoNormalizedRect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;
        public MFVideoNormalizedRect(float Left, float Top, float Right, float Bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFARGB
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbAlpha;
        public MFARGB(byte RgbBlue, byte RgbGreen, byte RgbRed, byte RgbAlpha)
        {
            rgbBlue = RgbBlue;
            rgbGreen = RgbGreen;
            rgbRed = RgbRed;
            rgbAlpha = RgbAlpha;
        }
    }

    [ComImport]
    [Guid("ad4c1b00-4bf7-422f-9175-756693d9130d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFByteStream
    {
        HRESULT GetCapabilities(out int pdwCapabilities);
        HRESULT GetLength(out ulong pqwLength);
        HRESULT SetLength(ulong qwLength);
        HRESULT GetCurrentPosition(out ulong pqwPosition);
        HRESULT SetCurrentPosition(ulong qwPosition);
        HRESULT IsEndOfStream(out bool pfEndOfStream);
        HRESULT Read(out IntPtr pb, uint cb, out uint pcbRead);
        HRESULT BeginRead(out IntPtr pb, uint cb, IMFAsyncCallback pCallback, IntPtr punkState);
        HRESULT EndRead(IMFAsyncResult pResult, out uint pcbRead);
        HRESULT Write(IntPtr pb, uint cb, out uint pcbWritten);
        HRESULT BeginWrite(IntPtr pb, uint cb, IMFAsyncCallback pCallback, IntPtr punkState);
        HRESULT EndWrite(IMFAsyncResult pResult, out uint pcbWritten);
        HRESULT Seek(MFBYTESTREAM_SEEK_ORIGIN SeekOrigin, long llSeekOffset, int dwSeekFlags, out ulong pqwCurrentPosition);
        HRESULT Flush();
        HRESULT Close();
    }

    [ComImport]
    [Guid("a27003cf-2354-4f2a-8d6a-ab7cff15437e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAsyncCallback
    {
        HRESULT GetParameters(out int pdwFlags, out int pdwQueue);
        HRESULT Invoke(IMFAsyncResult pAsyncResult);
    }

    [ComImport]
    [Guid("ac6b7889-0740-4d51-8619-905994a55cc6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAsyncResult
    {
        //HRESULT GetState(out IUnknown **ppunkState);
        HRESULT GetState(out IntPtr ppunkState);
        HRESULT GetStatus();
        HRESULT SetStatus(HRESULT hrStatus);
        //HRESULT GetObject(out IUnknown ppObject);
        HRESULT GetObject(out IntPtr ppObject);
        //IUnknown* GetStateNoAddRef();
        IntPtr GetStateNoAddRef();
    }

    public enum MFBYTESTREAM_SEEK_ORIGIN
    {
        msoBegin = 0,
        msoCurrent = (msoBegin + 1)
    }

    [ComImport]
    [Guid("3137f1cd-fe5e-4805-a5d8-fb477448cb3d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSinkWriter
    {
        HRESULT AddStream(IMFMediaType pTargetMediaType, out int pdwStreamIndex);
        [PreserveSig]
        HRESULT SetInputMediaType(int dwStreamIndex, IMFMediaType pInputMediaType, IMFAttributes pEncodingParameters);
        HRESULT BeginWriting();
        [PreserveSig]
        HRESULT WriteSample(int dwStreamIndex, IMFSample pSample);
        HRESULT SendStreamTick(int dwStreamIndex, long llTimestamp);
        HRESULT PlaceMarker(int dwStreamIndex, IntPtr pvContext);
        HRESULT NotifyEndOfSegment(int dwStreamIndex);
        HRESULT Flush(int dwStreamIndex);
        [PreserveSig]
        HRESULT Finalize();
        HRESULT GetServiceForStream(int dwStreamIndex, ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
        HRESULT GetStatistics(int dwStreamIndex, out MF_SINK_WRITER_STATISTICS pStats);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MF_SINK_WRITER_STATISTICS
    {
        int cb;
        long llLastTimestampReceived;
        long llLastTimestampEncoded;
        long llLastTimestampProcessed;
        long llLastStreamTickReceived;
        long llLastSinkSampleRequest;
        ulong qwNumSamplesReceived;
        ulong qwNumSamplesEncoded;
        ulong qwNumSamplesProcessed;
        ulong qwNumStreamTicksReceived;
        int dwByteCountQueued;
        ulong qwByteCountProcessed;
        int dwNumOutstandingSinkSampleRequests;
        int dwAverageSampleRateReceived;
        int dwAverageSampleRateEncoded;
        int dwAverageSampleRateProcessed;
    }

    [ComImport]
    [Guid("44ae0fa8-ea31-4109-8d2e-4cae4997c555")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaType : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //new HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT GetMajorType(out Guid pguidMajorType);
        HRESULT IsCompressedFormat(out bool pfCompressed);
        HRESULT IsEqual(IMFMediaType pIMediaType, out int pdwFlags);
        HRESULT GetRepresentation(ref Guid guidRepresentation, out IntPtr ppvRepresentation);
        HRESULT FreeRepresentation(ref Guid guidRepresentation, IntPtr pvRepresentation);
    }

    [ComImport]
    [Guid("c40a00f2-b93a-4d80-ae8c-5a1c634f58e4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSample : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //new HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT GetSampleFlags(out int pdwSampleFlags);
        HRESULT SetSampleFlags(int dwSampleFlags);
        HRESULT GetSampleTime(out long phnsSampleTime);
        HRESULT SetSampleTime(long hnsSampleTime);
        HRESULT GetSampleDuration(out long phnsSampleDuration);
        HRESULT SetSampleDuration(long hnsSampleDuration);
        HRESULT GetBufferCount(out int pdwBufferCount);
        HRESULT GetBufferByIndex(int dwIndex, out IMFMediaBuffer ppBuffer);
        HRESULT ConvertToContiguousBuffer(out IMFMediaBuffer ppBuffer);
        HRESULT AddBuffer(IMFMediaBuffer pBuffer);
        HRESULT RemoveBufferByIndex(int dwIndex);
        HRESULT RemoveAllBuffers();
        HRESULT GetTotalLength(out int pcbTotalLength);
        HRESULT CopyToBuffer(IMFMediaBuffer pBuffer);
    }

    [Guid("045FA593-8799-42b8-BC8D-8968C6453507")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaBuffer
    {
        HRESULT Lock(out IntPtr ppbBuffer, out int pcbMaxLength, out int pcbCurrentLength);
        HRESULT Unlock();
        HRESULT GetCurrentLength(out int pcbCurrentLength);
        HRESULT SetCurrentLength(int cbCurrentLength);
        HRESULT GetMaxLength(out int pcbMaxLength);
    }


    [ComImport]
    [Guid("7DC9D5F9-9ED9-44ec-9BBF-0600BB589FBB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMF2DBuffer
    {
        HRESULT Lock2D(out IntPtr ppbScanline0, out uint plPitch);
        HRESULT Unlock2D();
        HRESULT GetScanline0AndPitch(out IntPtr pbScanline0, out uint plPitch);
        HRESULT IsContiguousFormat(out bool pfIsContiguous);
        HRESULT GetContiguousLength(out uint pcbLength);
        HRESULT ContiguousCopyTo(out IntPtr pbDestBuffer, uint cbDestBuffer);
        HRESULT ContiguousCopyFrom(IntPtr pbSrcBuffer, uint cbSrcBuffer);
    }

    [ComImport]
    [Guid("33ae5ea6-4316-436f-8ddd-d73d22f829ec")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMF2DBuffer2 : IMF2DBuffer
    {
        #region IMF2DBuffer
        new HRESULT Lock2D(out IntPtr ppbScanline0, out uint plPitch);
        new HRESULT Unlock2D();
        new HRESULT GetScanline0AndPitch(out IntPtr pbScanline0, out uint plPitch);
        new HRESULT IsContiguousFormat(out bool pfIsContiguous);
        new HRESULT GetContiguousLength(out uint pcbLength);
        new HRESULT ContiguousCopyTo(out IntPtr pbDestBuffer, uint cbDestBuffer);
        new HRESULT ContiguousCopyFrom(IntPtr pbSrcBuffer, uint cbSrcBuffer);
        #endregion

        HRESULT Lock2DSize(MF2DBuffer_LockFlags lockFlags, out IntPtr ppbScanline0, out int plPitch, out IntPtr ppbBufferStart, out uint pcbBufferLength);
        HRESULT Copy2DTo(IMF2DBuffer2 pDestBuffer);
    }

    public enum MF2DBuffer_LockFlags
    {
        MF2DBuffer_LockFlags_LockTypeMask = ((0x1 | 0x2) | 0x3),
        MF2DBuffer_LockFlags_Read = 0x1,
        MF2DBuffer_LockFlags_Write = 0x2,
        MF2DBuffer_LockFlags_ReadWrite = 0x3,
        MF2DBuffer_LockFlags_ForceDWORD = 0x7fffffff
    }

    [ComImport]
    [Guid("83015ead-b1e6-40d0-a98a-37145ffe1ad1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineEx : IMFMediaEngine
    {
        #region IMFMediaEngine
        new HRESULT GetError(out IMFMediaError ppError);
        new HRESULT SetErrorCode(MF_MEDIA_ENGINE_ERR error);
        new HRESULT SetSourceElements(IMFMediaEngineSrcElements pSrcElements);
        [PreserveSig]
        new HRESULT SetSource(string pUrl);
        new HRESULT GetCurrentSource(out string ppUrl);
        [PreserveSig]
        new byte GetNetworkState();
        [PreserveSig]
        new MF_MEDIA_ENGINE_PRELOAD GetPreload();
        new HRESULT SetPreload(MF_MEDIA_ENGINE_PRELOAD Preload);
        new HRESULT GetBuffered(out IMFMediaTimeRange ppBuffered);
        [PreserveSig]
        new HRESULT Load();
        new HRESULT CanPlayType(string type, out MF_MEDIA_ENGINE_CANPLAY pAnswer);
        [PreserveSig]
        new MF_MEDIA_ENGINE_READY GetReadyState();
        [PreserveSig]
        new bool IsSeeking();
        [PreserveSig]
        new double GetCurrentTime();
        new HRESULT SetCurrentTime(double seekTime);
        [PreserveSig]
        new double GetStartTime();
        [PreserveSig]
        new double GetDuration();
        [PreserveSig]
        new bool IsPaused();
        [PreserveSig]
        new double GetDefaultPlaybackRate();
        new HRESULT SetDefaultPlaybackRate(double Rate);
        [PreserveSig]
        new double GetPlaybackRate();
        new HRESULT SetPlaybackRate(double Rate);
        new HRESULT GetPlayed(out IMFMediaTimeRange ppPlayed);
        new HRESULT GetSeekable(out IMFMediaTimeRange ppSeekable);
        [PreserveSig]
        new bool IsEnded();
        [PreserveSig]
        new bool GetAutoPlay();
        new HRESULT SetAutoPlay(bool AutoPlay);
        [PreserveSig]
        new bool GetLoop();
        new HRESULT SetLoop(bool Loop);
        new HRESULT Play();
        new HRESULT Pause();
        [PreserveSig]
        new bool GetMuted();
        new HRESULT SetMuted(bool Muted);
        [PreserveSig]
        new double GetVolume();
        new HRESULT SetVolume(double Volume);
        [PreserveSig]
        new bool HasVideo();
        [PreserveSig]
        new bool HasAudio();
        new HRESULT GetNativeVideoSize(out uint cx, out uint cy);
        new HRESULT GetVideoAspectRatio(out uint cx, out uint cy);
        new HRESULT Shutdown();
        [PreserveSig]
        new HRESULT TransferVideoFrame(IntPtr pDstSurf, ref MFVideoNormalizedRect pSrc, ref RECT pDst, ref MFARGB pBorderClr);
        [PreserveSig]
        new HRESULT OnVideoStreamTick(out long pPts);
        #endregion

        HRESULT SetSourceFromByteStream(IMFByteStream pByteStream, string pURL);
        HRESULT GetStatistics(MF_MEDIA_ENGINE_STATISTIC StatisticID, [Out, MarshalAs(UnmanagedType.Struct)] out PROPVARIANT pStatistic);
        HRESULT UpdateVideoStream(ref MFVideoNormalizedRect pSrc, ref RECT pDst, ref MFARGB pBorderClr);
        [PreserveSig]
        double GetBalance();
        HRESULT SetBalance(double balance);
        [PreserveSig]
        bool IsPlaybackRateSupported(double rate);
        [PreserveSig]
        HRESULT FrameStep(bool Forward);
        HRESULT GetResourceCharacteristics(out uint pCharacteristics);
        HRESULT GetPresentationAttribute(ref Guid guidMFAttribute, [Out, MarshalAs(UnmanagedType.Struct)] out PROPVARIANT pvValue);
        HRESULT GetNumberOfStreams(out uint pdwStreamCount);
        HRESULT GetStreamAttribute(uint dwStreamIndex, ref Guid guidMFAttribute, [Out, MarshalAs(UnmanagedType.Struct)] out PROPVARIANT pvValue);
        HRESULT GetStreamSelection(uint dwStreamIndex, out bool pEnabled);
        HRESULT SetStreamSelection(uint dwStreamIndex, bool Enabled);
        HRESULT ApplyStreamSelections();
        HRESULT IsProtected(out bool pProtected);
        HRESULT InsertVideoEffect(IntPtr pEffect, bool fOptional);
        HRESULT InsertAudioEffect(IntPtr pEffect, bool fOptional);
        HRESULT RemoveAllEffects();
        HRESULT SetTimelineMarkerTimer(double timeToFire);
        HRESULT GetTimelineMarkerTimer(out double pTimeToFire);
        HRESULT CancelTimelineMarkerTimer();
        [PreserveSig]
        bool IsStereo3D();
        HRESULT GetStereo3DFramePackingMode(out MF_MEDIA_ENGINE_S3D_PACKING_MODE packMode);
        HRESULT SetStereo3DFramePackingMode(MF_MEDIA_ENGINE_S3D_PACKING_MODE packMode);
        HRESULT GetStereo3DRenderMode(out MF3DVideoOutputType outputType);
        HRESULT SetStereo3DRenderMode(MF3DVideoOutputType outputType);
        HRESULT EnableWindowlessSwapchainMode(bool fEnable);
        HRESULT GetVideoSwapchainIntPtr(out IntPtr phSwapchain);
        HRESULT EnableHorizontalMirrorMode(bool fEnable);
        HRESULT GetAudioStreamCategory(out uint pCategory);
        HRESULT SetAudioStreamCategory(uint category);
        HRESULT GetAudioEndpointRole(out uint pRole);
        HRESULT SetAudioEndpointRole(uint role);
        HRESULT GetRealTimeMode(out bool pfEnabled);
        HRESULT SetRealTimeMode(bool fEnable);
        HRESULT SetCurrentTimeEx(double seekTime, MF_MEDIA_ENGINE_SEEK_MODE seekMode);
        HRESULT EnableTimeUpdateTimer(bool fEnableTimer);
    }

    public enum MF_MEDIA_ENGINE_STATISTIC
    {
        MF_MEDIA_ENGINE_STATISTIC_FRAMES_RENDERED = 0,
        MF_MEDIA_ENGINE_STATISTIC_FRAMES_DROPPED = 1,
        MF_MEDIA_ENGINE_STATISTIC_BYTES_DOWNLOADED = 2,
        MF_MEDIA_ENGINE_STATISTIC_BUFFER_PROGRESS = 3,
        MF_MEDIA_ENGINE_STATISTIC_FRAMES_PER_SECOND = 4,
        MF_MEDIA_ENGINE_STATISTIC_PLAYBACK_JITTER = 5,
        MF_MEDIA_ENGINE_STATISTIC_FRAMES_CORRUPTED = 6,
        MF_MEDIA_ENGINE_STATISTIC_TOTAL_FRAME_DELAY = 7
    }

    public enum MF_MEDIA_ENGINE_S3D_PACKING_MODE
    {
        MF_MEDIA_ENGINE_S3D_PACKING_MODE_NONE = 0,
        MF_MEDIA_ENGINE_S3D_PACKING_MODE_SIDE_BY_SIDE = 1,
        MF_MEDIA_ENGINE_S3D_PACKING_MODE_TOP_BOTTOM = 2
    }

    public enum MF_MEDIA_ENGINE_SEEK_MODE
    {
        MF_MEDIA_ENGINE_SEEK_MODE_NORMAL = 0,
        MF_MEDIA_ENGINE_SEEK_MODE_APPROXIMATE = 1
    }

    public enum MF3DVideoOutputType
    {
        MF3DVideoOutputType_BaseView = 0,
        MF3DVideoOutputType_Stereo = 1
    }

    [ComImport]
    [Guid("bf94c121-5b05-4e6f-8000-ba598961414d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTransform
    {
        HRESULT GetStreamLimits(out uint pdwInputMinimum, out uint pdwInputMaximum, out uint pdwOutputMinimum, out uint pdwOutputMaximum);
        HRESULT GetStreamCount(out uint pcInputStreams, out uint pcOutputStreams);
        HRESULT GetStreamIDs(uint dwInputIDArraySize, out uint pdwInputIDs, uint dwOutputIDArraySize, out uint pdwOutputIDs);
        HRESULT GetInputStreamInfo(uint dwInputStreamID, out MFT_INPUT_STREAM_INFO pStreamInfo);
        HRESULT GetOutputStreamInfo(uint dwOutputStreamID, out MFT_OUTPUT_STREAM_INFO pStreamInfo);
        HRESULT GetAttributes(out IMFAttributes pAttributes);
        HRESULT GetInputStreamAttributes(uint dwInputStreamID, out IMFAttributes pAttributes);
        HRESULT GetOutputStreamAttributes(uint dwOutputStreamID, out IMFAttributes pAttributes);
        HRESULT DeleteInputStream(uint dwStreamID);
        HRESULT AddInputStreams(uint cStreams, uint adwStreamIDs);
        HRESULT GetInputAvailableType(uint dwInputStreamID, uint dwTypeIndex, out IMFMediaType ppType);
        HRESULT GetOutputAvailableType(uint dwOutputStreamID, uint dwTypeIndex, out IMFMediaType ppType);
        HRESULT SetInputType(uint dwInputStreamID, IMFMediaType pType, uint dwFlags);
        HRESULT SetOutputType(uint dwOutputStreamID, IMFMediaType pType, uint dwFlags);
        HRESULT GetInputCurrentType(uint dwInputStreamID, out IMFMediaType ppType);
        HRESULT GetOutputCurrentType(uint dwOutputStreamID, out IMFMediaType ppType);
        HRESULT GetInputStatus(uint dwInputStreamID, out uint pdwFlags);
        HRESULT GetOutputStatus(out uint pdwFlags);
        HRESULT SetOutputBounds(long hnsLowerBound, long hnsUpperBound);
        HRESULT ProcessEvent(uint dwInputStreamID, IMFMediaEvent pEvent);
        HRESULT ProcessMessage(MFT_MESSAGE_TYPE eMessage, IntPtr ulParam);
        HRESULT ProcessInput(uint dwInputStreamID, IMFSample pSample, uint dwFlags);
        HRESULT ProcessOutput(uint dwFlags, uint cOutputBufferCount, ref MFT_OUTPUT_DATA_BUFFER pOutputSamples, out uint pdwStatus);
    }

    public enum MFT_INPUT_STREAM_INFO_FLAGS
    {
        MFT_INPUT_STREAM_WHOLE_SAMPLES = 0x1,
        MFT_INPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER = 0x2,
        MFT_INPUT_STREAM_FIXED_SAMPLE_SIZE = 0x4,
        MFT_INPUT_STREAM_HOLDS_BUFFERS = 0x8,
        MFT_INPUT_STREAM_DOES_NOT_ADDREF = 0x100,
        MFT_INPUT_STREAM_REMOVABLE = 0x200,
        MFT_INPUT_STREAM_OPTIONAL = 0x400,
        MFT_INPUT_STREAM_PROCESSES_IN_PLACE = 0x800
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct MFT_INPUT_STREAM_INFO
    {
        public long hnsMaxLatency;
        public uint dwFlags;
        public uint cbSize;
        public uint cbMaxLookahead;
        public uint cbAlignment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFT_OUTPUT_STREAM_INFO
    {
        public uint dwFlags;
        public uint cbSize;
        public uint cbAlignment;
    }

    public enum MFT_MESSAGE_TYPE
    {
        MFT_MESSAGE_COMMAND_FLUSH = 0,
        MFT_MESSAGE_COMMAND_DRAIN = 0x1,
        MFT_MESSAGE_SET_D3D_MANAGER = 0x2,
        MFT_MESSAGE_DROP_SAMPLES = 0x3,
        MFT_MESSAGE_COMMAND_TICK = 0x4,
        MFT_MESSAGE_NOTIFY_BEGIN_STREAMING = 0x10000000,
        MFT_MESSAGE_NOTIFY_END_STREAMING = 0x10000001,
        MFT_MESSAGE_NOTIFY_END_OF_STREAM = 0x10000002,
        MFT_MESSAGE_NOTIFY_START_OF_STREAM = 0x10000003,
        MFT_MESSAGE_NOTIFY_RELEASE_RESOURCES = 0x10000004,
        MFT_MESSAGE_NOTIFY_REACQUIRE_RESOURCES = 0x10000005,
        MFT_MESSAGE_NOTIFY_EVENT = 0x10000006,
        MFT_MESSAGE_COMMAND_SET_OUTPUT_STREAM_STATE = 0x10000007,
        MFT_MESSAGE_COMMAND_FLUSH_OUTPUT_STREAM = 0x10000008,
        MFT_MESSAGE_COMMAND_MARKER = 0x20000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFT_OUTPUT_DATA_BUFFER
    {
        public uint dwStreamID;
        public IMFSample pSample;
        public uint dwStatus;
        public IMFCollection pEvents;
    }

    [ComImport]
    [Guid("5BC8A76B-869A-46a3-9B03-FA218A66AEBE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCollection
    {
        HRESULT GetElementCount(out uint pcElements);
        HRESULT GetElement(uint dwElementIndex, out IntPtr ppUnkElement);
        HRESULT AddElement(IntPtr pUnkElement);
        HRESULT RemoveElement(uint dwElementIndex, out IntPtr ppUnkElement);
        HRESULT InsertElementAt(uint dwIndex, IntPtr pUnknown);
        HRESULT RemoveAllElements();
    }

    [ComImport]
    [Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEvent : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //new HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT GetType(out uint pmet);
        HRESULT GetExtendedType(out Guid pguidExtendedType);
        HRESULT GetStatus(out HRESULT phrStatus);
        HRESULT GetValue([Out, MarshalAs(UnmanagedType.Struct)] out PROPVARIANT pvValue);
    }

    [ComImport]
    [Guid("1f2a94c9-a3df-430d-9d0f-acd85ddc29af")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedText
    {
        HRESULT RegisterNotifications(IMFTimedTextNotify notify);
        [PreserveSig]
        HRESULT SelectTrack(uint trackId, bool selected);
        [PreserveSig]
        HRESULT AddDataSource(IMFByteStream byteStream, string label, string language, MF_TIMED_TEXT_TRACK_KIND kind, bool isDefault, out uint trackId);
        [PreserveSig]
        HRESULT AddDataSourceFromUrl(string url, string label, string language, MF_TIMED_TEXT_TRACK_KIND kind, bool isDefault, out uint trackId);
        [PreserveSig]
        HRESULT AddTrack(string label, string language, MF_TIMED_TEXT_TRACK_KIND kind, out IMFTimedTextTrack track);
        HRESULT RemoveTrack(IMFTimedTextTrack track);
        HRESULT GetCueTimeOffset(out double offset);
        HRESULT SetCueTimeOffset(double offset);
        HRESULT GetTracks(out IMFTimedTextTrackList tracks);
        HRESULT GetActiveTracks(out IMFTimedTextTrackList activeTracks);
        HRESULT GetTextTracks(out IMFTimedTextTrackList textTracks);
        HRESULT GetMetadataTracks(out IMFTimedTextTrackList metadataTracks);
        HRESULT SetInBandEnabled(bool enabled);
        [PreserveSig]
        bool IsInBandEnabled();
    }

    public enum MF_TIMED_TEXT_TRACK_KIND
    {
        MF_TIMED_TEXT_TRACK_KIND_UNKNOWN = 0,
        MF_TIMED_TEXT_TRACK_KIND_SUBTITLES = 1,
        MF_TIMED_TEXT_TRACK_KIND_CAPTIONS = 2,
        MF_TIMED_TEXT_TRACK_KIND_METADATA = 3
    }

    [ComImport]
    [Guid("df6b87b6-ce12-45db-aba7-432fe054e57d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextNotify
    {
        void TrackAdded(uint trackId);
        void TrackRemoved(uint trackId);
        void TrackSelected(uint trackId, bool selected);
        void TrackReadyStateChanged(MF_TIMED_TEXT_TRACK_READY_STATE trackId);
        void Error(MF_TIMED_TEXT_ERROR_CODE errorCode, HRESULT extendedErrorCode, uint sourceTrackId);
        void Cue(MF_TIMED_TEXT_CUE_EVENT cueEvent, double currentTime, IMFTimedTextCue cue);
        void Reset();
    }

    public enum MF_TIMED_TEXT_ERROR_CODE
    {
        MF_TIMED_TEXT_ERROR_CODE_NOERROR = 0,
        MF_TIMED_TEXT_ERROR_CODE_FATAL = 1,
        MF_TIMED_TEXT_ERROR_CODE_DATA_FORMAT = 2,
        MF_TIMED_TEXT_ERROR_CODE_NETWORK = 3,
        MF_TIMED_TEXT_ERROR_CODE_INTERNAL = 4
    }

    public enum MF_TIMED_TEXT_CUE_EVENT
    {
        MF_TIMED_TEXT_CUE_EVENT_ACTIVE = 0,
        MF_TIMED_TEXT_CUE_EVENT_INACTIVE = (MF_TIMED_TEXT_CUE_EVENT_ACTIVE + 1),
        MF_TIMED_TEXT_CUE_EVENT_CLEAR = (MF_TIMED_TEXT_CUE_EVENT_INACTIVE + 1)
    }

    [ComImport]
    [Guid("8822c32d-654e-4233-bf21-d7f2e67d30d4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextTrack
    {
        [PreserveSig]
        uint GetId();
        HRESULT GetLabel([Out, MarshalAs(UnmanagedType.LPWStr)] out string label);
        HRESULT SetLabel(string label);
        HRESULT GetLanguage([Out, MarshalAs(UnmanagedType.LPWStr)] out string language);
        [PreserveSig]
        MF_TIMED_TEXT_TRACK_KIND GetTrackKind();
        [PreserveSig]
        bool IsInBand();
        HRESULT GetInBandMetadataTrackDispatchType(out string dispatchType);
        [PreserveSig]
        bool IsActive();
        [PreserveSig]
        MF_TIMED_TEXT_ERROR_CODE GetErrorCode();
        HRESULT GetExtendedErrorCode();
        HRESULT GetDataFormat(out Guid format);
        [PreserveSig]
        MF_TIMED_TEXT_TRACK_READY_STATE GetReadyState();
        HRESULT GetCueList(out IMFTimedTextCueList cues);
    }

    public enum MF_TIMED_TEXT_TRACK_READY_STATE
    {
        MF_TIMED_TEXT_TRACK_READY_STATE_NONE = 0,
        MF_TIMED_TEXT_TRACK_READY_STATE_LOADING = (MF_TIMED_TEXT_TRACK_READY_STATE_NONE + 1),
        MF_TIMED_TEXT_TRACK_READY_STATE_LOADED = (MF_TIMED_TEXT_TRACK_READY_STATE_LOADING + 1),
        MF_TIMED_TEXT_TRACK_READY_STATE_ERROR = (MF_TIMED_TEXT_TRACK_READY_STATE_LOADED + 1)
    }

    [ComImport]
    [Guid("23ff334c-442c-445f-bccc-edc438aa11e2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextTrackList
    {
        [PreserveSig]
        uint GetLength();
        HRESULT GetTrack(uint index, out IMFTimedTextTrack track);
        HRESULT GetTrackById(uint trackId, out IMFTimedTextTrack track);
    }

    [ComImport]
    [Guid("ad128745-211b-40a0-9981-fe65f166d0fd")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextCueList
    {
        [PreserveSig]
        uint GetLength();
        HRESULT GetCueByIndex(uint index, out IMFTimedTextCue cue);
        HRESULT GetCueById(uint id, out IMFTimedTextCue cue);
        HRESULT GetCueByOriginalId(string originalId, out IMFTimedTextCue cue);
        HRESULT AddTextCue(double start, double duration, string text, out IMFTimedTextCue cue);
        HRESULT AddDataCue(double start, double duration, IntPtr data, uint dataSize, IMFTimedTextCue cue);
        HRESULT RemoveCue(IMFTimedTextCue cue);
    }

    [ComImport]
    [Guid("1e560447-9a2b-43e1-a94c-b0aaabfbfbc9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextCue
    {
        [PreserveSig]
        uint GetId();
        [PreserveSig]
        //HRESULT GetOriginalId(out IntPtr originalId);
        //HRESULT GetOriginalId(StringBuilder originalId);
        HRESULT GetOriginalId([Out, MarshalAs(UnmanagedType.LPWStr)] out string originalId);
        [PreserveSig]
        MF_TIMED_TEXT_TRACK_KIND GetCueKind();
        [PreserveSig]
        double GetStartTime();
        [PreserveSig]
        double GetDuration();
        [PreserveSig]
        uint GetTrackId();
        HRESULT GetData(out IMFTimedTextBinary data);
        HRESULT GetRegion(out IMFTimedTextRegion region);
        HRESULT GetStyle(out IMFTimedTextStyle style);
        [PreserveSig]
        uint GetLineCount();
        HRESULT GetLine(uint index, out IMFTimedTextFormattedText line);
    }

    [ComImport]
    [Guid("4ae3a412-0545-43c4-bf6f-6b97a5c6c432")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextBinary
    {
        HRESULT GetData(out IntPtr data, out uint length);
    }

    [ComImport]
    [Guid("c8d22afc-bc47-4bdf-9b04-787e49ce3f58")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextRegion
    {
        HRESULT GetName([Out, MarshalAs(UnmanagedType.LPWStr)] out string name);
        HRESULT GetPosition(out double pX, out double pY, out MF_TIMED_TEXT_UNIT_TYPE unitType);
        HRESULT GetExtent(out double pWidth, out double pHeight, out MF_TIMED_TEXT_UNIT_TYPE unitType);
        HRESULT GetBackgroundColor(out MFARGB bgColor);
        HRESULT GetWritingMode(out MF_TIMED_TEXT_WRITING_MODE writingMode);
        HRESULT GetDisplayAlignment(out MF_TIMED_TEXT_DISPLAY_ALIGNMENT displayAlign);
        HRESULT GetLineHeight(out double pLineHeight, out MF_TIMED_TEXT_UNIT_TYPE unitType);
        HRESULT GetClipOverflow(out bool clipOverflow);
        HRESULT GetPadding(out double before, out double start, out double after, out double end, out MF_TIMED_TEXT_UNIT_TYPE unitType);
        HRESULT GetWrap(out bool wrap);
        HRESULT GetZIndex(out int zIndex);
        HRESULT GetScrollMode(out MF_TIMED_TEXT_SCROLL_MODE scrollMode);
    }

    public enum MF_TIMED_TEXT_UNIT_TYPE
    {
        MF_TIMED_TEXT_UNIT_TYPE_PIXELS = 0,
        MF_TIMED_TEXT_UNIT_TYPE_PERCENTAGE = 1
    }

    public enum MF_TIMED_TEXT_WRITING_MODE
    {
        MF_TIMED_TEXT_WRITING_MODE_LRTB = 0,
        MF_TIMED_TEXT_WRITING_MODE_RLTB = 1,
        MF_TIMED_TEXT_WRITING_MODE_TBRL = 2,
        MF_TIMED_TEXT_WRITING_MODE_TBLR = 3,
        MF_TIMED_TEXT_WRITING_MODE_LR = 4,
        MF_TIMED_TEXT_WRITING_MODE_RL = 5,
        MF_TIMED_TEXT_WRITING_MODE_TB = 6
    }

    public enum MF_TIMED_TEXT_DISPLAY_ALIGNMENT
    {
        MF_TIMED_TEXT_DISPLAY_ALIGNMENT_BEFORE = 0,
        MF_TIMED_TEXT_DISPLAY_ALIGNMENT_AFTER = 1,
        MF_TIMED_TEXT_DISPLAY_ALIGNMENT_CENTER = 2
    }

    public enum MF_TIMED_TEXT_SCROLL_MODE
    {
        MF_TIMED_TEXT_SCROLL_MODE_POP_ON = 0,
        MF_TIMED_TEXT_SCROLL_MODE_ROLL_UP = 1
    }

    [ComImport]
    [Guid("09b2455d-b834-4f01-a347-9052e21c450e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextStyle
    {
        HRESULT GetName([Out, MarshalAs(UnmanagedType.LPWStr)] out string name);
        [PreserveSig]
        bool IsExternal(); HRESULT GetFontFamily(out string fontFamily);
        HRESULT GetFontSize(out double fontSize, out MF_TIMED_TEXT_UNIT_TYPE unitType);
        HRESULT GetColor(out MFARGB color);
        HRESULT GetBackgroundColor(out MFARGB bgColor);
        HRESULT GetShowBackgroundAlways(out bool showBackgroundAlways);
        HRESULT GetFontStyle(out MF_TIMED_TEXT_FONT_STYLE fontStyle);
        HRESULT GetBold(out bool bold);
        HRESULT GetRightToLeft(out bool rightToLeft);
        HRESULT GetTextAlignment(out MF_TIMED_TEXT_ALIGNMENT textAlign);
        HRESULT GetTextDecoration(out uint textDecoration);
        HRESULT GetTextOutline(out MFARGB color, out double thickness, out double blurRadius, out MF_TIMED_TEXT_UNIT_TYPE unitType);
    }

    public enum MF_TIMED_TEXT_FONT_STYLE
    {
        MF_TIMED_TEXT_FONT_STYLE_NORMAL = 0,
        MF_TIMED_TEXT_FONT_STYLE_OBLIQUE = 1,
        MF_TIMED_TEXT_FONT_STYLE_ITALIC = 2
    }

    public enum MF_TIMED_TEXT_ALIGNMENT
    {
        MF_TIMED_TEXT_ALIGNMENT_START = 0,
        MF_TIMED_TEXT_ALIGNMENT_END = 1,
        MF_TIMED_TEXT_ALIGNMENT_CENTER = 2
    }

    [ComImport]
    [Guid("e13af3c1-4d47-4354-b1f5-e83ae0ecae60")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTimedTextFormattedText
    {
        [PreserveSig]
        HRESULT GetText([Out, MarshalAs(UnmanagedType.LPWStr)] out string text);
        [return: MarshalAs(UnmanagedType.U4)]
        [PreserveSig]
        uint GetSubformattingCount();
        [PreserveSig]
        HRESULT GetSubformatting(uint index, out uint firstChar, out uint charLength, out IMFTimedTextStyle style);
    }

    [ComImport]
    [Guid("A3F675D5-6119-4f7f-A100-1D8B280F0EFB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoProcessorControl
    {
        HRESULT SetBorderColor(ref MFARGB pBorderColor);
        HRESULT SetSourceRectangle(ref RECT pSrcRect);
        HRESULT SetDestinationRectangle(ref RECT pDstRect);
        HRESULT SetMirror(MF_VIDEO_PROCESSOR_MIRROR eMirror);
        HRESULT SetRotation(MF_VIDEO_PROCESSOR_ROTATION eRotation);
        HRESULT SetConstrictionSize(Windows.Foundation.Size pConstrictionSize);
    }

    public enum MF_VIDEO_PROCESSOR_MIRROR
    {
        MIRROR_NONE = 0,
        MIRROR_HORIZONTAL = 1,
        MIRROR_VERTICAL = 2
    }

    public enum MF_VIDEO_PROCESSOR_ROTATION
    {
        ROTATION_NONE = 0,
        ROTATION_NORMAL = 1
    }

    [ComImport]
    [Guid("BDE633D3-E1DC-4a7f-A693-BBAE399C4A20")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoProcessorControl2 : IMFVideoProcessorControl
    {
        #region IMFVideoProcessorControl
        new HRESULT SetBorderColor(ref MFARGB pBorderColor);
        new HRESULT SetSourceRectangle(ref RECT pSrcRect);
        new HRESULT SetDestinationRectangle(ref RECT pDstRect);
        new HRESULT SetMirror(MF_VIDEO_PROCESSOR_MIRROR eMirror);
        new HRESULT SetRotation(MF_VIDEO_PROCESSOR_ROTATION eRotation);
        new HRESULT SetConstrictionSize(Windows.Foundation.Size pConstrictionSize);
        #endregion

        HRESULT SetRotationOverride(MFVideoRotationFormat uiRotation);
        HRESULT EnableHardwareEffects(bool fEnabled);
        [PreserveSig]
        HRESULT GetSupportedHardwareEffects(out uint puiSupport);
    }

    [ComImport]
    [Guid("2424B3F2-EB23-40f1-91AA-74BDDEEA0883")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoProcessorControl3 : IMFVideoProcessorControl2
    {
        #region IMFVideoProcessorControl2
        #region IMFVideoProcessorControl
        new HRESULT SetBorderColor(ref MFARGB pBorderColor);
        new HRESULT SetSourceRectangle(ref RECT pSrcRect);
        new HRESULT SetDestinationRectangle(ref RECT pDstRect);
        new HRESULT SetMirror(MF_VIDEO_PROCESSOR_MIRROR eMirror);
        new HRESULT SetRotation(MF_VIDEO_PROCESSOR_ROTATION eRotation);
        new HRESULT SetConstrictionSize(Windows.Foundation.Size pConstrictionSize);
        #endregion

        new HRESULT SetRotationOverride(MFVideoRotationFormat uiRotation);
        new HRESULT EnableHardwareEffects(bool fEnabled);
        [PreserveSig]
        new HRESULT GetSupportedHardwareEffects(out uint puiSupport);
        #endregion

        HRESULT GetNaturalOutputType(out IMFMediaType ppType);
        HRESULT EnableSphericalVideoProcessing(bool fEnable, MFVideoSphericalFormat eFormat, MFVideoSphericalProjectionMode eProjectionMode);
        HRESULT SetSphericalVideoProperties(float X, float Y, float Z, float W, float fieldOfView);
        HRESULT SetOutputDevice(IntPtr pOutputDevice);
    }

    public enum MFVideoSphericalFormat
    {
        MFVideoSphericalFormat_Unsupported = 0,
        MFVideoSphericalFormat_Equirectangular = 1,
        MFVideoSphericalFormat_CubeMap = 2,
        MFVideoSphericalFormat_3DMesh = 3
    }

    public enum MFVideoSphericalProjectionMode
    {
        MFVideoSphericalProjectionMode_Spherical = 0,
        MFVideoSphericalProjectionMode_Flat = (MFVideoSphericalProjectionMode_Spherical + 1)
    }

    public enum MFVideoRotationFormat
    {
        MFVideoRotationFormat_0 = 0,
        MFVideoRotationFormat_90 = 90,
        MFVideoRotationFormat_180 = 180,
        MFVideoRotationFormat_270 = 270,
    }

    [ComImport]
    [Guid("6AB0000C-FECE-4d1f-A2AC-A9573530656E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoProcessor
    {
        HRESULT GetAvailableVideoProcessorModes(ref uint lpdwNumProcessingModes, out Guid pVideoProcessingModes);
    }

    [ComImport]
    [Guid("7FEE9E9A-4A89-47a6-899C-B6A53A70FB67")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFActivate : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT ActivateObject(ref Guid riid, out IntPtr ppv);        
        HRESULT ShutdownObject();        
        HRESULT DetachObject();        
    }

    [ComImport]
    [Guid("6ef2a660-47c0-4666-b13d-cbb717f2fa2c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSink
    {
        HRESULT GetCharacteristics(out uint pdwCharacteristics);
        HRESULT AddStreamSink(uint dwStreamSinkIdentifier, IMFMediaType pMediaType, out IMFStreamSink ppStreamSink);
        HRESULT RemoveStreamSink(uint dwStreamSinkIdentifier);
        HRESULT GetStreamSinkCount(out uint pcStreamSinkCount);
        HRESULT GetStreamSinkByIndex(uint dwIndex, out IMFStreamSink ppStreamSink);
        HRESULT GetStreamSinkById(out uint dwStreamSinkIdentifier, out IMFStreamSink ppStreamSink);
        HRESULT SetPresentationClock(IMFPresentationClock pPresentationClock);
        HRESULT GetPresentationClock(out IMFPresentationClock ppPresentationClock);
        HRESULT Shutdown();
    }

    [ComImport]
    [Guid("2CD0BD52-BCD5-4B89-B62C-EADC0C031E7D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEventGenerator
    {
        HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
        HRESULT BeginGetEvent(IMFAsyncCallback pCallback, IntPtr punkState);
        HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
        HRESULT QueueEvent(uint met, ref Guid guidExtendedType, HRESULT hrStatus, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvValue);
    }

    [ComImport]
    [Guid("0A97B3CF-8E7C-4a3d-8F8C-0C843DC247FB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFStreamSink : IMFMediaEventGenerator
    {
        #region IMFMediaEventGenerator
        new HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
        new HRESULT BeginGetEvent(IMFAsyncCallback pCallback, IntPtr punkState);
        new HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
        new HRESULT QueueEvent(uint met, ref Guid guidExtendedType, HRESULT hrStatus, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvValue);
        #endregion

        HRESULT GetMediaSink(out IMFMediaSink ppMediaSink);
        HRESULT GetIdentifier(out uint pdwIdentifier);
        HRESULT GetMediaTypeHandler(out IMFMediaTypeHandler ppHandler);
        HRESULT ProcessSample(IMFSample pSample);
        HRESULT PlaceMarker(MFSTREAMSINK_MARKER_TYPE eMarkerType, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvarMarkerValue, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvarContextValue);
        HRESULT Flush();
    }

    public enum MFSTREAMSINK_MARKER_TYPE
    {
        MFSTREAMSINK_MARKER_DEFAULT = 0,
        MFSTREAMSINK_MARKER_ENDOFSEGMENT = (MFSTREAMSINK_MARKER_DEFAULT + 1),
        MFSTREAMSINK_MARKER_TICK = (MFSTREAMSINK_MARKER_ENDOFSEGMENT + 1),
        MFSTREAMSINK_MARKER_EVENT = (MFSTREAMSINK_MARKER_TICK + 1)
    }

    [ComImport]
    [Guid("e93dcf6c-4b07-4e1e-8123-aa16ed6eadf5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaTypeHandler
    {
        HRESULT IsMediaTypeSupported(IMFMediaType pMediaType, out IMFMediaType ppMediaType);
        HRESULT GetMediaTypeCount(out uint pdwTypeCount);
        HRESULT GetMediaTypeByIndex(uint dwIndex, out IMFMediaType ppType);
        HRESULT SetCurrentMediaType(IMFMediaType pMediaType);
        HRESULT GetCurrentMediaType(out IMFMediaType ppMediaType);
        HRESULT GetMajorType(out Guid pguidMajorType);
    }

    [ComImport]
    [Guid("2eb1e945-18b8-4139-9b1a-d5d584818530")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFClock
    {
        HRESULT GetClockCharacteristics(out uint pdwCharacteristics);
        HRESULT GetCorrelatedTime(uint dwReserved, out long pllClockTime, out long phnsSystemTime);
        HRESULT GetContinuityKey(out uint pdwContinuityKey);
        HRESULT GetState(uint dwReserved, out MFCLOCK_STATE peClockState);
        HRESULT GetProperties(out MFCLOCK_PROPERTIES pClockProperties);
    }

    public enum MFCLOCK_STATE
    {
        MFCLOCK_STATE_INVALID = 0,
        MFCLOCK_STATE_RUNNING = (MFCLOCK_STATE_INVALID + 1),
        MFCLOCK_STATE_STOPPED = (MFCLOCK_STATE_RUNNING + 1),
        MFCLOCK_STATE_PAUSED = (MFCLOCK_STATE_STOPPED + 1)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFCLOCK_PROPERTIES
    {
        public UInt64 qwCorrelationRate;
        public Guid guidClockId;
        public uint dwClockFlags;
        public UInt64 qwClockFrequency;
        public uint dwClockTolerance;
        public uint dwClockJitter;
    }

    [ComImport]
    [Guid("868CE85C-8EA9-4f55-AB82-B009A910A805")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFPresentationClock : IMFClock
    {
        #region IMFClock
        new HRESULT GetClockCharacteristics(out uint pdwCharacteristics);
        new HRESULT GetCorrelatedTime(uint dwReserved, out long pllClockTime, out long phnsSystemTime);
        new HRESULT GetContinuityKey(out uint pdwContinuityKey);
        new HRESULT GetState(uint dwReserved, out MFCLOCK_STATE peClockState);
        new HRESULT GetProperties(out MFCLOCK_PROPERTIES pClockProperties);
        #endregion

        HRESULT SetTimeSource(IMFPresentationTimeSource pTimeSource);
        HRESULT GetTimeSource(out IMFPresentationTimeSource ppTimeSource);
        HRESULT GetTime(out long phnsClockTime);
        HRESULT AddClockStateSink(IMFClockStateSink pStateSink);
        HRESULT RemoveClockStateSink(IMFClockStateSink pStateSink);
        HRESULT Start(long llClockStartOffset);
        HRESULT Stop();
        HRESULT Pause();
    }

    [ComImport]
    [Guid("7FF12CCE-F76F-41c2-863B-1666C8E5E139")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFPresentationTimeSource : IMFClock
    {
        #region IMFClock
        new HRESULT GetClockCharacteristics(out uint pdwCharacteristics);
        new HRESULT GetCorrelatedTime(uint dwReserved, out long pllClockTime, out long phnsSystemTime);
        new HRESULT GetContinuityKey(out uint pdwContinuityKey);
        new HRESULT GetState(uint dwReserved, out MFCLOCK_STATE peClockState);
        new HRESULT GetProperties(out MFCLOCK_PROPERTIES pClockProperties);
        #endregion

        HRESULT GetUnderlyingClock(out IMFClock ppClock);
    }

    [ComImport]
    [Guid("F6696E82-74F7-4f3d-A178-8A5E09C3659F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFClockStateSink
    {
        HRESULT OnClockStart(long hnsSystemTime, long llClockStartOffset);
        HRESULT OnClockStop(long hnsSystemTime);
        HRESULT OnClockPause(long hnsSystemTime);
        HRESULT OnClockRestart(long hnsSystemTime);
        HRESULT OnClockSetRate(long hnsSystemTime, float flRate);
    }

    [ComImport]
    [Guid("FBE5A32D-A497-4b61-BB85-97B1A848A6E3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSourceResolver
    {
        HRESULT CreateObjectFromURL(string pwszURL, uint dwFlags, IPropertyStore pProps, out MF_OBJECT_TYPE pObjectType, out IntPtr ppObject);
        HRESULT CreateObjectFromByteStream(IMFByteStream pByteStream, string pwszURL, uint dwFlags,
          IPropertyStore pProps, out MF_OBJECT_TYPE pObjectType, out IntPtr ppObject);
        HRESULT BeginCreateObjectFromURL(string pwszURL, uint dwFlags, IPropertyStore pProps, out IntPtr ppIntPtrCancelCookie, IMFAsyncCallback pCallback, IntPtr punkState);
        HRESULT EndCreateObjectFromURL(IMFAsyncResult pResult, out MF_OBJECT_TYPE pObjectType, out IntPtr ppObject);
        HRESULT BeginCreateObjectFromByteStream(IMFByteStream pByteStream, string pwszURL,
          uint dwFlags, IPropertyStore pProps, out IntPtr ppIntPtrCancelCookie, IMFAsyncCallback pCallback, IntPtr punkState);
        HRESULT EndCreateObjectFromByteStream(IMFAsyncResult pResult, out MF_OBJECT_TYPE pObjectType, out IntPtr ppObject);
        HRESULT CancelObjectCreation(IntPtr pIntPtrCancelCookie);
    }

    [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        HRESULT GetCount([Out] out uint propertyCount);
        HRESULT GetAt([In] uint propertyIndex, [Out, MarshalAs(UnmanagedType.Struct)] out PROPERTYKEY key);
        HRESULT GetValue([In, MarshalAs(UnmanagedType.Struct)] ref PROPERTYKEY key, [Out, MarshalAs(UnmanagedType.Struct)] out PROPVARIANT pv);
        HRESULT SetValue([In, MarshalAs(UnmanagedType.Struct)] ref PROPERTYKEY key, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pv);
        HRESULT Commit();
    }

    public enum MF_OBJECT_TYPE
    {
        MF_OBJECT_MEDIASOURCE = 0,
        MF_OBJECT_BYTESTREAM = (MF_OBJECT_MEDIASOURCE + 1),
        MF_OBJECT_INVALID = (MF_OBJECT_BYTESTREAM + 1)
    }

    [ComImport]
    [Guid("279a808d-aec7-40c8-9c6b-a6b492c78a66")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSource : IMFMediaEventGenerator
    {
        #region IMFMediaEventGenerator
        new HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
        new HRESULT BeginGetEvent(IMFAsyncCallback pCallback, IntPtr punkState);
        new HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
        new HRESULT QueueEvent(uint met, ref Guid guidExtendedType, HRESULT hrStatus, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvValue);
        #endregion

        HRESULT GetCharacteristics(out uint pdwCharacteristics);
        HRESULT CreatePresentationDescriptor(out IMFPresentationDescriptor ppPresentationDescriptor);
        HRESULT Start(IMFPresentationDescriptor pPresentationDescriptor, ref Guid pguidTimeFormat, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pvarStartPosition);
        HRESULT Stop();
        HRESULT Pause();
        HRESULT Shutdown();
    }

    [ComImport]
    [Guid("03cb2711-24d7-4db6-a17f-f3a7a479a536")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFPresentationDescriptor : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //new HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT GetStreamDescriptorCount(out uint pdwDescriptorCount);
        HRESULT GetStreamDescriptorByIndex(uint dwIndex, out bool pfSelected, out IMFStreamDescriptor ppDescriptor);
        HRESULT SelectStream(uint dwDescriptorIndex);
        HRESULT DeselectStream(uint dwDescriptorIndex);
        HRESULT Clone(out IMFPresentationDescriptor ppPresentationDescriptor);
    }

    [ComImport]
    [Guid("56c03d9c-9dbb-45f5-ab4b-d80f47c05938")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFStreamDescriptor : IMFAttributes
    {
        #region IMFAttributes
        new HRESULT GetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT GetItemType(ref Guid guidKey, out MF_ATTRIBUTE_TYPE pType);
        new HRESULT CompareItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value, out bool pbResult);
        new HRESULT Compare(IMFAttributes pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);
        new HRESULT GetUINT32(ref Guid guidKey, out uint punValue);
        new HRESULT GetUINT64(ref Guid guidKey, out ulong punValue);
        new HRESULT GetDouble(ref Guid guidKey, out double pfValue);
        new HRESULT GetGUID(ref Guid guidKey, out Guid pguidValue);
        new HRESULT GetStringLength(ref Guid guidKey, out uint pcchLength);
        new HRESULT GetString(ref Guid guidKey, out string pwszValue, uint cchBufSize, ref uint pcchLength);
        new HRESULT GetAllocatedString(ref Guid guidKey, out string ppwszValue, out uint pcchLength);
        new HRESULT GetBlobSize(ref Guid guidKey, out uint pcbBlobSize);
        new HRESULT GetBlob(ref Guid guidKey, out char pBuf, uint cbBufSize, ref uint pcbBlobSize);
        new HRESULT GetAllocatedBlob(ref Guid guidKey, out char ppBuf, out uint pcbSize);
        new HRESULT GetUnknown(ref Guid guidKey, ref Guid riid, out IntPtr ppv);
        new HRESULT SetItem(ref Guid guidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT Value);
        new HRESULT DeleteItem(ref Guid guidKey);
        new HRESULT DeleteAllItems();
        new HRESULT SetUINT32(ref Guid guidKey, uint unValue);
        new HRESULT SetUINT64(ref Guid guidKey, ulong unValue);
        new HRESULT SetDouble(ref Guid guidKey, double fValue);
        new HRESULT SetGUID(ref Guid guidKey, ref Guid guidValue);
        new HRESULT SetString(ref Guid guidKey, string wszValue);
        new HRESULT SetBlob(ref Guid guidKey, char pBuf, uint cbBufSize);
        //new HRESULT SetUnknown(ref Guid guidKey, IUnknown pUnknown = null);
        new HRESULT SetUnknown(ref Guid guidKey, IntPtr pUnknown);
        new HRESULT LockStore();
        new HRESULT UnlockStore();
        new HRESULT GetCount(out uint pcItems);
        new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In, MarshalAs(UnmanagedType.Struct)] ref PROPVARIANT pValue);
        new HRESULT CopyAllItems(IMFAttributes pDest = null);
        #endregion

        HRESULT GetStreamIdentifier(out uint pdwStreamIdentifier);
        HRESULT GetMediaTypeHandler(out IMFMediaTypeHandler ppMediaTypeHandler);
    }






    internal class MediaEngineTools
    {      
        public static Guid CLSID_MFMediaEngineClassFactory = new Guid("b44392da-499b-446b-a4cb-005fead0e6d5");
        public static Guid CLSID_VideoProcessorMFT = new Guid("88753b26-5b24-49bd-b2e7-0c445c78c982");
        public static Guid CLSID_CColorControlDmo = new Guid("798059f0-89ca-4160-b325-aeb48efe4f9a");
        public static Guid MF_XVP_PLAYBACK_MODE = new Guid("3c5d293f-ad67-4e29-af12-cf3e238acce9");
        public static Guid MF_MEDIASOURCE_SERVICE = new Guid("f09992f7-9fba-4c4a-a37f-8c47b4e1dfe7");
        public static Guid MF_BYTESTREAM_CONTENT_TYPE = new Guid("fc358289-3cb6-460c-a424-b6681260375a");

        public static Guid MF_MEDIA_ENGINE_CALLBACK = new Guid("c60381b8-83a4-41f8-a3d0-de05076849a9");
        public static Guid MF_MEDIA_ENGINE_DXGI_MANAGER = new Guid("065702da-1094-486d-8617-ee7cc4ee4648");
        public static Guid MF_MEDIA_ENGINE_EXTENSION = new Guid("3109fd46-060d-4b62-8dcf-faff811318d2");
        public static Guid MF_MEDIA_ENGINE_PLAYBACK_HWND = new Guid("d988879b-67c9-4d92-baa7-6eadd446039d");
        public static Guid MF_MEDIA_ENGINE_OPM_HWND = new Guid("a0be8ee7-0572-4f2c-a801-2a151bd3e726");
        public static Guid MF_MEDIA_ENGINE_PLAYBACK_VISUAL = new Guid("6debd26f-6ab9-4d7e-b0ee-c61a73ffad15");
        public static Guid MF_MEDIA_ENGINE_COREWINDOW = new Guid("fccae4dc-0b7f-41c2-9f96-4659948acddc");
        public static Guid MF_MEDIA_ENGINE_VIDEO_OUTPUT_FORMAT = new Guid("5066893c-8cf9-42bc-8b8a-472212e52726");
        public static Guid MF_MEDIA_ENGINE_CONTENT_PROTECTION_FLAGS = new Guid("e0350223-5aaf-4d76-a7c3-06de70894db4");
        public static Guid MF_MEDIA_ENGINE_CONTENT_PROTECTION_MANAGER = new Guid("fdd6dfaa-bd85-4af3-9e0f-a01d539d876a");
        public static Guid MF_MEDIA_ENGINE_AUDIO_ENDPOINT_ROLE = new Guid("d2cb93d1-116a-44f2-9385-f7d0fda2fb46");
        public static Guid MF_MEDIA_ENGINE_AUDIO_CATEGORY = new Guid("c8d4c51d-350e-41f2-ba46-faebbb0857f6");
        public static Guid MF_MEDIA_ENGINE_STREAM_CONTAINS_ALPHA_CHANNEL = new Guid("5cbfaf44-d2b2-4cfb-80a7-d429c74c789d");
        public static Guid MF_MEDIA_ENGINE_BROWSER_COMPATIBILITY_MODE = new Guid("4e0212e2-e18f-41e1-95e5-c0e7e9235bc3");
        public static Guid MF_MEDIA_ENGINE_BROWSER_COMPATIBILITY_MODE_IE9 = new Guid("052c2d39-40c0-4188-ab86-f828273b7522");
        public static Guid MF_MEDIA_ENGINE_BROWSER_COMPATIBILITY_MODE_IE10 = new Guid("11a47afd-6589-4124-b312-6158ec517fc3");
        public static Guid MF_MEDIA_ENGINE_BROWSER_COMPATIBILITY_MODE_IE11 = new Guid("1cf1315f-ce3f-4035-9391-16142f775189");
        public static Guid MF_MEDIA_ENGINE_BROWSER_COMPATIBILITY_MODE_IE_EDGE = new Guid("a6f3e465-3aca-442c-a3f0-ad6ddad839ae");
        public static Guid MF_MEDIA_ENGINE_COMPATIBILITY_MODE = new Guid("3ef26ad4-dc54-45de-b9af-76c8c66bfa8e");
        public static Guid MF_MEDIA_ENGINE_COMPATIBILITY_MODE_WWA_EDGE = new Guid("15b29098-9f01-4e4d-b65a-c06c6c89da2a");
        public static Guid MF_MEDIA_ENGINE_COMPATIBILITY_MODE_WIN10 = new Guid("5b25e089-6ca7-4139-a2cb-fcaab39552a3");
        public static Guid MF_MEDIA_ENGINE_SOURCE_RESOLVER_CONFIG_STORE = new Guid("0ac0c497-b3c4-48c9-9cde-bb8ca2442ca3");
        public static Guid MF_MEDIA_ENGINE_TRACK_ID = new Guid("65bea312-4043-4815-8eab-44dce2ef8f2a");
        public static Guid MF_MEDIA_ENGINE_TELEMETRY_APPLICATION_ID = new Guid("1e7b273b-a7e4-402a-8f51-c48e88a2cabc");
        public static Guid MF_MEDIA_ENGINE_SYNCHRONOUS_CLOSE = new Guid("c3c2e12f-7e0e-4e43-b91c-dc992ccdfa5e");
        public static Guid MF_MEDIA_ENGINE_MEDIA_PLAYER_MODE = new Guid("3ddd8d45-5aa1-4112-82e5-36f6a2197e6e");
        public static Guid MF_MEDIA_ENGINE_TIMEDTEXT = new Guid("805EA411-92E0-4E59-9B6E-5C7D7915E64F");
        public static Guid MF_MEDIA_ENGINE_CONTINUE_ON_CODEC_ERROR = new Guid("dbcdb7f9-48e4-4295-b70d-d518234eeb38");
        public static Guid MF_MEDIA_ENGINE_EME_CALLBACK = new Guid("494553a7-a481-4cb7-bec5-380903513731");

        public static Guid MFMediaType_Default = new Guid("81A412E6-8103-4B06-857F-1862781024AC");
        public static Guid MFMediaType_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");
        public static Guid MFMediaType_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");
        public static Guid MFMediaType_Protected = new Guid("7b4b6fe6-9d04-4494-be14-7e0bd076c8e4");
        public static Guid MFMediaType_SAMI = new Guid("e69669a0-3dcd-40cb-9e2e-3708387c0616");
        public static Guid MFMediaType_Script = new Guid("72178C22-E45B-11D5-BC2A-00B0D0F3F4AB");
        public static Guid MFMediaType_Image = new Guid("72178C23-E45B-11D5-BC2A-00B0D0F3F4AB");
        public static Guid MFMediaType_HTML = new Guid("72178C24-E45B-11D5-BC2A-00B0D0F3F4AB");
        public static Guid MFMediaType_Binary = new Guid("72178C25-E45B-11D5-BC2A-00B0D0F3F4AB");
        public static Guid MFMediaType_FileTransfer = new Guid("72178C26-E45B-11D5-BC2A-00B0D0F3F4AB");
        public static Guid MFMediaType_Stream = new Guid("e436eb83-524f-11ce-9f53-0020af0ba770");
        public static Guid MFMediaType_MultiplexedFrames = new Guid("6ea542b0-281f-4231-a464-fe2f5022501c");
        public static Guid MFMediaType_Subtitle = new Guid("a6d13581-ed50-4e65-ae08-26065576aacc");
        public static Guid MFMediaType_Perception = new Guid("597ff6f9-6ea2-4670-85b4-ea84073fe940");
        public static Guid MFImageFormat_JPEG = new Guid("19e4a5aa-5662-4fc5-a0c0-1758028e1057");
        public static Guid MFImageFormat_RGB32 = new Guid("00000016-0000-0010-8000-00aa00389b71");
        public static Guid MFStreamFormat_MPEG2Transport = new Guid("e06d8023-db46-11cf-b4d1-00805f6cbbea");
        public static Guid MFStreamFormat_MPEG2Program = new Guid("263067d1-d330-45dc-b669-34d986e4e3e1");
        public static Guid AM_MEDIA_TYPE_REPRESENTATION = new Guid("e2e42ad2-132c-491e-a268-3c7c2dca181f");
        public static Guid FORMAT_MFVideoFormat = new Guid("aed4ab2d-7326-43cb-9464-c879cab9c43d");

        public static Guid MFVideoFormat_WMV3 = new Guid("33564D57-0000-0010-8000-00AA00389B71");
        public static Guid MFVideoFormat_MP4V = new Guid("5634504D-0000-0010-8000-00AA00389B71");
        public static Guid MFVideoFormat_RGB32 = new Guid("00000016-0000-0010-8000-00AA00389B71"); 
        public static Guid MFVideoFormat_H264 = new Guid("34363248-0000-0010-8000-00AA00389B71");
        public static Guid MFVideoFormat_H265 = new Guid("35363248-0000-0010-8000-00aa00389b71");
        
        public static Guid MFAudioFormat_AAC = new Guid("00001610-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_ADTS = new Guid("00001600-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_Dolby_AC3_SPDIF = new Guid("00000092-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_DRM = new Guid("00000009-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_DTS = new Guid("00000008-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_Float = new Guid("00000003-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_MP3 = new Guid("00000055-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_MPEG = new Guid("00000050-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_MSP1 = new Guid("0000000a-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_PCM = new Guid("00000001-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_WMASPDIF = new Guid("00000164-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_WMAudio_Lossless = new Guid("00000163-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_WMAudioV8 = new Guid("00000161-0000-0010-8000-00aa00389b71");
        public static Guid MFAudioFormat_WMAudioV9 = new Guid("00000162-0000-0010-8000-00aa00389b71");

        public static Guid MFTranscodeContainerType_ASF = new Guid("430f6f6e-b6bf-4fc1-a0bd-9ee46eee2afb");
        public static Guid MFTranscodeContainerType_MPEG4 = new Guid("dc6cd05d-b9d0-40ef-bd35-fa622c1ab28a");
        public static Guid MFTranscodeContainerType_MP3 = new Guid("e438b912-83f1-4de6-9e3a-9ffbc6dd24d1");
        public static Guid MFTranscodeContainerType_3GP = new Guid("34c50167-4472-4f34-9ea0-c49fbacf037d");

        public static Guid MF_MT_MAJOR_TYPE = new Guid("48eba18e-f8c9-4687-bf11-0a74c9f96a8f");
        public static Guid MF_MT_SUBTYPE = new Guid("f7e34c9a-42e8-4714-b74b-cb29d72c35e5");
        public static Guid MF_MT_ALL_SAMPLES_INDEPENDENT = new Guid("c9173739-5e56-461c-b713-46fb995cb95f");
        public static Guid MF_MT_FIXED_SIZE_SAMPLES = new Guid("b8ebefaf-b718-4e04-b0a9-116775e3321b");
        public static Guid MF_MT_COMPRESSED = new Guid("3afd0cee-18f2-4ba5-a110-8bea502e1f92");
        public static Guid MF_MT_SAMPLE_SIZE = new Guid("dad3ab78-1990-408b-bce2-eba673dacc10");
        public static Guid MF_MT_WRAPPED_TYPE = new Guid("4d3f7b23-d02f-4e6c-9bee-e4bf2c6c695d");

        public static Guid MF_MT_AUDIO_NUM_CHANNELS = new Guid("37e48bf5-645e-4c5b-89de-ada9e29b696a");
        public static Guid MF_MT_AUDIO_SAMPLES_PER_SECOND = new Guid("5faeeae7-0290-4c31-9e8a-c534f68d9dba");
        public static Guid MF_MT_AUDIO_FLOAT_SAMPLES_PER_SECOND = new Guid("fb3b724a-cfb5-4319-aefe-6e42b2406132");
        public static Guid MF_MT_AUDIO_AVG_BYTES_PER_SECOND = new Guid("1aab75c8-cfef-451c-ab95-ac034b8e1731");
        public static Guid MF_MT_AUDIO_BLOCK_ALIGNMENT = new Guid("322de230-9eeb-43bd-ab7a-ff412251541d");
        public static Guid MF_MT_AUDIO_BITS_PER_SAMPLE = new Guid("f2deb57f-40fa-4764-aa33-ed4f2d1ff669");
        public static Guid MF_MT_AUDIO_VALID_BITS_PER_SAMPLE = new Guid("d9bf8d6a-9530-4b7c-9ddf-ff6fd58bbd06");
        public static Guid MF_MT_AUDIO_SAMPLES_PER_BLOCK = new Guid("aab15aac-e13a-4995-9222-501ea15c6877");
        public static Guid MF_MT_AUDIO_CHANNEL_MASK = new Guid("55fb5765-644a-4caf-8479-938983bb1588");
        public static Guid MF_MT_AUDIO_FOLDDOWN_MATRIX = new Guid("9d62927c-36be-4cf2-b5c4-a3926e3e8711");
        public static Guid MF_MT_AUDIO_WMADRC_PEAKREF = new Guid("9d62927d-36be-4cf2-b5c4-a3926e3e8711");
        public static Guid MF_MT_AUDIO_WMADRC_PEAKTARGET = new Guid("9d62927e-36be-4cf2-b5c4-a3926e3e8711");
        public static Guid MF_MT_AUDIO_WMADRC_AVGREF = new Guid("9d62927f-36be-4cf2-b5c4-a3926e3e8711");
        public static Guid MF_MT_AUDIO_WMADRC_AVGTARGET = new Guid("9d629280-36be-4cf2-b5c4-a3926e3e8711");
        public static Guid MF_MT_AUDIO_PREFER_WAVEFORMATEX = new Guid("a901aaba-e037-458a-bdf6-545be2074042");
        public static Guid MF_MT_AAC_PAYLOAD_TYPE = new Guid("bfbabe79-7434-4d1c-94f0-72a3b9e17188");
        public static Guid MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION = new Guid("7632f0e6-9538-4d61-acda-ea29c8c14456");

        public static Guid MF_MT_FRAME_SIZE = new Guid("1652c33d-d6b2-4012-b834-72030849a37d");
        public static Guid MF_MT_FRAME_RATE = new Guid("c459a2e8-3d2c-4e44-b132-fee5156c7bb0");
        public static Guid MF_MT_PIXEL_ASPECT_RATIO = new Guid("c6376a1e-8d0a-4027-be45-6d9a0ad39bb6");
        public static Guid MF_MT_DRM_FLAGS = new Guid("8772f323-355a-4cc7-bb78-6d61a048ae82");
        public static Guid MF_MT_PAD_CONTROL_FLAGS = new Guid("4d0e73e5-80ea-4354-a9d0-1176ceb028ea");
        public static Guid MF_MT_SOURCE_CONTENT_HINT = new Guid("68aca3cc-22d0-44e6-85f8-28167197fa38");
        public static Guid MF_MT_INTERLACE_MODE = new Guid("e2724bb8-e676-4806-b4b2-a8d6efb44ccd");
        public static Guid MF_MT_TRANSFER_FUNCTION = new Guid("5fb0fce9-be5c-4935-a811-ec838f8eed93");
        public static Guid MF_MT_CUSTOM_VIDEO_PRIMARIES = new Guid("47537213-8cfb-4722-aa34-fbc9e24d77b8");
        public static Guid MF_MT_YUV_MATRIX = new Guid("3e23d450-2c75-4d25-a00e-b91670d12327");
        public static Guid MF_MT_GEOMETRIC_APERTURE = new Guid("66758743-7e5f-400d-980a-aa8596c85696");
        public static Guid MF_MT_MINIMUM_DISPLAY_APERTURE = new Guid("d7388766-18fe-48c6-a177-ee894867c8c4");
        public static Guid MF_MT_PAN_SCAN_APERTURE = new Guid("79614dde-9187-48fb-b8c7-4d52689de649");
        public static Guid MF_MT_PAN_SCAN_ENABLED = new Guid("4b7f6bc3-8b13-40b2-a993-abf630b8204e");
        public static Guid MF_MT_AVG_BITRATE = new Guid("20332624-fb0d-4d9e-bd0d-cbf6786c102e");
        public static Guid MF_MT_AVG_BIT_ERROR_RATE = new Guid("799cabd6-3508-4db4-a3c7-569cd533deb1");
        public static Guid MF_MT_MAX_KEYFRAME_SPACING = new Guid("c16eb52b-73a1-476f-8d62-839d6a020652");
        public static Guid MF_MT_MPEG4_SAMPLE_DESCRIPTION = new Guid("261e9d83-9529-4b8f-a111-8b9c950a81a9");
        public static Guid MF_MT_MPEG4_CURRENT_SAMPLE_ENTRY = new Guid("9aa7e155-b64a-4c1d-a500-455d600b6560");
        public static Guid MF_MT_VIDEO_CHROMA_SITING = new Guid("65df2370-c773-4c33-aa64-843e068efb0c");
        public static Guid MF_MT_VIDEO_PRIMARIES = new Guid("dbfbe4d7-0740-4ee0-8192-850ab0e21935");
        public static Guid MF_MT_VIDEO_LIGHTING = new Guid("53a0529c-890b-4216-8bf9-599367ad6d20");
        public static Guid MF_MT_VIDEO_NOMINAL_RANGE = new Guid("c21b8ee5-b956-4071-8daf-325edf5cab11");

        public static Guid CLSID_BmpCodec = new Guid("557CF400-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_JpegCodec = new Guid("557CF401-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_GifCodec = new Guid("557CF402-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_EMFCodec = new Guid("557CF403-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_WMFCodec = new Guid("557CF404-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_TiffCodec = new Guid("557CF405-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_PngCodec = new Guid("557CF406-1A04-11D3-9A73-0000F81EF32E");
        public static Guid CLSID_IcoCodec = new Guid("557CF407-1A04-11D3-9A73-0000F81EF32E");

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFStartup(uint Version, uint dwFlags = 0);

        public const int MF_SDK_VERSION = 0x0002;
        public const int MF_API_VERSION = 0x0070; // This value is unused in the Win7 release and left at its Vista release value
        public const int MF_VERSION = (MF_SDK_VERSION << 16 | MF_API_VERSION);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFShutdown();

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateAttributes(out IMFAttributes ppMFAttributes, uint cInitialSize);

        [DllImport("Mfreadwrite.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT MFCreateSinkWriterFromURL(string pwszOutputURL, IMFByteStream pByteStream, IMFAttributes pAttributes, out IMFSinkWriter ppSinkWriter);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateMediaType(out IMFMediaType ppMFType);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateMemoryBuffer(uint cbMaxLength, out IMFMediaBuffer ppBuffer);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCopyImage(IntPtr pDest, int lDestStride, IntPtr pSrc, int lSrcStride, uint dwWidthInBytes, uint dwLines);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateSample(out IMFSample ppIMFSample);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateMFByteStreamOnStream(System.Runtime.InteropServices.ComTypes.IStream pStream, out IMFByteStream ppByteStream);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateMFByteStreamOnStreamEx(IntPtr punkStream, out IMFByteStream ppByteStream);

        [DllImport("Shlwapi.dll", SetLastError = true)]
        public static extern System.Runtime.InteropServices.ComTypes.IStream SHCreateMemStream(byte[] pInit, uint cbInit);

        [DllImport("Mf.dll", SetLastError = true)]
        public static extern HRESULT MFGetService(IntPtr punkObject, [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppvObject);

        [DllImport("Mfplat.dll", SetLastError = true)]
        public static extern HRESULT MFCreateDXGISurfaceBuffer([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,  IntPtr punkSurface, uint uSubresourceIndex,
            bool fBottomUpWhenLinear, out IMFMediaBuffer ppBuffer);

        private static ulong Pack2UINT32AsUINT64(uint unHigh, uint unLow)
        {
            return ((ulong)unHigh << 32) | unLow;
        }

        private static HRESULT MFSetAttribute2UINT32asUINT64(IMFAttributes pAttributes, ref Guid guidKey, uint unHigh32, uint unLow32)
        {
            return pAttributes.SetUINT64(guidKey, Pack2UINT32AsUINT64(unHigh32, unLow32));
        }

        public static HRESULT MFSetAttributeSize(IMFAttributes pAttributes, ref Guid guidKey, uint unWidth, uint unHeight)
        {
            return MFSetAttribute2UINT32asUINT64(pAttributes, ref guidKey, unWidth, unHeight);
        }

        public static HRESULT MFSetAttributeRatio(IMFAttributes pAttributes, ref Guid guidKey, uint unNumerator, uint unDenominator)
        {
            return MFSetAttribute2UINT32asUINT64(pAttributes, ref guidKey, unNumerator, unDenominator);
        }

        public const int DXGI_USAGE_SHADER_INPUT = 0x00000010;
        public const int DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x00000020;
        public const int DXGI_USAGE_BACK_BUFFER = 0x00000040;
        public const int DXGI_USAGE_SHARED = 0x00000080;
        public const int DXGI_USAGE_READ_ONLY = 0x00000100;
        public const int DXGI_USAGE_DISCARD_ON_PRESENT = 0x00000200;
        public const int DXGI_USAGE_UNORDERED_ACCESS = 0x00000400;
      
        public enum MF_MEDIA_ENGINE_CREATEFLAGS : uint
        {
            MF_MEDIA_ENGINE_AUDIOONLY = 0x1,
            MF_MEDIA_ENGINE_WAITFORSTABLE_STATE = 0x2,
            MF_MEDIA_ENGINE_FORCEMUTE = 0x4,
            MF_MEDIA_ENGINE_REAL_TIME_MODE = 0x8,
            MF_MEDIA_ENGINE_DISABLE_LOCAL_PLUGINS = 0x10,
            MF_MEDIA_ENGINE_CREATEFLAGS_MASK = 0x1f
        }

        public enum MF_MEDIA_ENGINE_ERR
        {
            MF_MEDIA_ENGINE_ERR_NOERROR = 0,
            MF_MEDIA_ENGINE_ERR_ABORTED = 1,
            MF_MEDIA_ENGINE_ERR_NETWORK = 2,
            MF_MEDIA_ENGINE_ERR_DECODE = 3,
            MF_MEDIA_ENGINE_ERR_SRC_NOT_SUPPORTED = 4,
            MF_MEDIA_ENGINE_ERR_ENCRYPTED = 5
        };

        public enum MFVideoInterlaceMode : uint
        {
            MFVideoInterlace_Unknown = 0,
            MFVideoInterlace_Progressive = 2,
            MFVideoInterlace_FieldInterleavedUpperFirst = 3,
            MFVideoInterlace_FieldInterleavedLowerFirst = 4,
            MFVideoInterlace_FieldSingleUpper = 5,
            MFVideoInterlace_FieldSingleLower = 6,
            MFVideoInterlace_MixedInterlaceOrProgressive = 7,
            MFVideoInterlace_Last = (MFVideoInterlace_MixedInterlaceOrProgressive + 1),
            MFVideoInterlace_ForceDWORD = 0x7fffffff
        }
    }
}
