#ifndef _SERVER_WRITER_ADAPTER_H_
#define _SERVER_WRITER_ADAPTER_H_

#include "functioncodes.h"
#include "binarymessage/binarymessage_write_adapter.h"
#include "support/support.h"
#include "log/log.h"



class ServerWriterAdapter : public BinaryMessage_Write_Adapter
{
public:
  ServerWriterAdapter(BinaryMessage& inmem) : BinaryMessage_Write_Adapter(inmem)
  {
  }

  bool ENCODE_ServerSendMessegeServerID( const UINT16 t_id )
  {
    LOGINFO("Encoding ID Resonse\n");
    AddInt2u( SERVER_SEND_MESSAGESERVER_ID);
    AddInt2u( t_id );
    return true;
  }

};
#endif
