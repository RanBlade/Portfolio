#ifndef _SERVER_BINARYMESSEGE_READER_H_
#define _SERVER_BINARYMESSEGE_READER_H_

#include "support/support.h"
#include "binarymessage/binarymessage_reader_adapter.h"

class ServerReaderAdapter : public BinaryMessage_Reader_Adapter
{
private:
    UINT16 m_functionCode;
public:
    ServerReaderAdapter(BinaryMessage& inmem) : BinaryMessage_Reader_Adapter(inmem) ,
                                                m_functionCode(0)
    {
        GetInt2u(m_functionCode);
    }
    UINT16 GetFunctionCode(){ return m_functionCode; }

    int DECODE_ServerRequestMessegeServerID();
    int DECODE_ServerForwardMessegeState();
    int DECODE_AdminCreateEvent(UINT16& t_code , std::string& t_string)
    {
      GetInt2u(t_code);
      GetStr(t_string);
    }
      



};
#endif
