using System;
using System.IO;
using System.Reflection;
using System.Text;
using SimDbg;

namespace SimU8engine;

public class CSimU8main
{
    private struct TraceLastMemDatT
    {
        public uint data;

        public byte byte_h_l;
    }

    private struct SimPeriInfo
    {
        public struct intInfoT
        {
            public ushort vec_adr;

            public ushort ie_adr;

            public ushort ie_bit;

            public ushort irq_adr;

            public ushort irq_bit;

            public byte[] sym_name;

            public void InitSymName()
            {
                sym_name = new byte[32];
            }
        }

        public byte[] dll_name;

        public string dllstr;

        public string dll_name_st;

        public ushort start_adr;

        public int int_num;

        public intInfoT[] intInfo;

        public void InitIntInfo()
        {
            dll_name = new byte[260];
            intInfo = new intInfoT[128];
            intInfoT[] array = intInfo;
            for (int i = 0; i < array.Length; i++)
            {
                ((intInfoT)array[i]).InitSymName();
            }
        }
    }

    public struct INTNUM
    {
        public uint int_req1;

        public uint int_req2;

        public uint int_req3;

        public uint int_req4;
    }

    public const int TRACESIZE = 262144;

    public const int INTNAME_LENGTH = 20;

    public const int MAX_IO_DATASZ = 260;

    public const int MAX_DLL_NAME_LEN = 256;

    public const string IO_SECNAME_FILEINFO = "FILE_INFO";

    public const string IO_SECNAME_CORE = "CORE";

    public const string IO_SECNAME_COPRO = "COPRO";

    public const string IO_SECNAME_PERIPHERAL = "PERIPHERAL";

    public const string IO_SECNAME_MEMORY = "MEMORY";

    public const string IO_SECNAME_DEBUG = "DEBUG";

    public const string IO_SECNAME_STOP_HALT_INFO = "STOP_HALT_INFO";

    public const ushort BRK_AD = 0;

    public const ushort BRK_DT_8 = 17;

    public const ushort BRK_ADDT_8 = 18;

    public const ushort BRK_DT_16 = 33;

    public const ushort BRK_ADDT_16 = 34;

    public const ushort BRK_DT_32 = 49;

    public const ushort BRK_ADDT_32 = 50;

    public const ushort BRK_AD_P = 256;

    public const ushort BRK_DT_8_P = 273;

    public const ushort BRK_ADDT_8_P = 274;

    public const ushort BRK_DT_16_P = 289;

    public const ushort BRK_ADDT_16_P = 290;

    public const ushort BRK_DT_32_P = 305;

    public const ushort BRK_ADDT_32_P = 306;

    public const byte SW_Lushort = 0;

    public const byte SW_ushort = 1;

    public const byte SW_byte = 2;

    public const byte WB_ushort = 0;

    public const byte WB_byte = 1;

    public const int SIM_RUN = 0;

    public const int SIM_HALT = 1;

    public const int SIM_STOP = 2;

    public const int SIM_UNDEFINEINST = 3;

    public const int SIM_ERROR = 4;

    public const int SIM_BP_BREAK = 5;

    public const int SIM_STEP_BREAK = 6;

    public const int SIM_BREAK = 7;

    public const int SIM_ADDRESS_BREAK = 8;

    public const int SIM_RAMDM_BREAK = 9;

    public const int SIM_FORCE_BREAK = 10;

    public const int SIM_TRACE_BREAK = 11;

    public const int SIM_PWDOWN_BREAK = 12;

    public const int SIM_ROMNA_BREAK = 13;

    public const int SIM_RAMNA_BREAK = 14;

    public const int SIM_RUN_ADRSSKIP = 15;

    public const int SIM_FORCE_BREAKRQ = 16;

    public const byte BIT_ADRSBRK = 1;

    public const byte BIT_RAMDMBRK = 2;

    public const uint BreakCond_PD = 1u;

    public const uint BreakCond_TF = 2u;

    public const uint BreakCond_XP = 4u;

    public const uint BreakCond_BP = 8u;

    public const uint BreakCond_NROM = 16u;

    public const uint BreakCond_NRAM = 32u;

    public const ushort none = 0;

    public const ushort powerDown = 1;

    public const ushort tmpAdrs = 2;

    public const ushort permBreakPoint = 4;

    public const ushort ramMatch = 8;

    public const ushort step = 64;

    public const ushort forceByUser = 128;

    public const ushort traceMemFull = 256;

    public const ushort externalSignal = 512;

    public const ushort romNAaccess = 1024;

    public const ushort ramNAaccess = 2048;

    public const uint CORE_REV_TYPE1 = 0u;

    public const uint CORE_REV_TYPE2 = 1u;

    public const uint CORE_REV_TYPE3 = 2u;

    public const uint CORE_REV_TYPE4 = 3u;

    public const int rCM = 12289;

    public const int rDM = 12305;

    public const int rGM = 12321;

    public const int rIEM = 12323;

    public const byte R0 = 0;

    public const byte PC = 16;

    public const byte LR = 17;

    public const byte ELR1 = 18;

    public const byte ELR2 = 19;

    public const byte ELR3 = 20;

    public const byte CSR = 21;

    public const byte LCSR = 22;

    public const byte ECSR1 = 23;

    public const byte ECSR2 = 24;

    public const byte ECSR3 = 25;

    public const byte PSW = 26;

    public const byte EPSW1 = 27;

    public const byte EPSW2 = 28;

    public const byte EPSW3 = 29;

    public const byte SP = 30;

    public const byte EA = 31;

    public const byte CR0 = 32;

    public const byte CR15 = 47;

    public const byte C = 48;

    public const byte Z = 49;

    public const byte S = 50;

    public const byte OV = 51;

    public const byte MIE = 52;

    public const byte HC = 53;

    public const byte ELEVEL = 54;

    public const byte PSWEND = 55;

    private uint m_STPACP = 61448u;

    private uint m_SBYCON = 61449u;

    private uint m_Halt_Jdg_bit_mask = 1u;

    private uint m_Halt_Jdg_val = 1u;

    private uint m_Stop_Jdg_bit_mask = 2u;

    private uint m_Stop_Jdg_val = 2u;

    private uint m_HaltH_Jdg_bit_mask = 1u;

    private uint m_HaltH_Jdg_val = 1u;

    private uint m_StopD_Jdg_bit_mask = 2u;

    private uint m_StopD_Jdg_val = 2u;

    private uint m_HaltC_Jdg_bit_mask = 1u;

    private uint m_HaltC_Jdg_val = 1u;

    private uint m_StopSV1_Jdg_bit_mask = 2u;

    private uint m_StopSV1_Jdg_val = 2u;

    private uint m_HaltSV1_Jdg_bit_mask = 1u;

    private uint m_HaltSV1_Jdg_val = 1u;

    private uint m_StopSV2_Jdg_bit_mask = 2u;

    private uint m_StopSV2_Jdg_val = 2u;

    public const byte MAX_INTERRUPT_NUM = 64;

    private bool m_IdIsEmpty;

    private bool m_IfIsEmpty;

    public int m_CycleCountEnable;

    private CDasmu8 m_dasmu8 = new CDasmu8();

    private CRegister m_Reg = new CRegister();

    private byte m_DSR;

    private byte m_DsrPrefixFlag;

    private int m_DefaultCode;

    private byte m_STPACPflag;

    public byte m_SimRunFlag;

    private bool m_Initflag;

    private uint m_gcount;

    private int m_Logflag;

    private uint m_CodeStartAdr;

    private uint m_CodeEndAdr;

    private uint m_DataStartAdr;

    private uint m_DataEndAdr;

    private uint m_RomWinStartAdr;

    private uint m_RomWinEndAdr;

    private CStdioFileW f1 = new CStdioFileW();

    private byte m_InterruptNum;

    private readonly INTERRUPTTABLE[] m_InterruptInfo;

    private int m_retNextPC;

    private uint m_RomNA_brk;

    private uint m_RamNA_brk;

    private uint m_NextPC;

    private uint m_BreakPC;

    private readonly uint m_TraceNum;

    private uint m_TraceLastMemAdr;

    private TraceLastMemDatT m_TraceLastMemDat;

    private uint m_TraceBreakPointer;

    private ulong m_beforeCycleCounter;

    private byte m_StepFlag;

    private byte m_StepInst;

    private readonly bool m_bInstEAplus;

    private readonly uint[] TrcRamAdr;

    private readonly uint[] RdRamAdr;

    private readonly byte[] TrcRamDat;

    private byte TrcRamCnt;

    private readonly byte[] RdRamDat;

    private byte RdRamCnt;

    private uint m_CvCntAddressStart;

    private uint m_CvCntAddressEnd;

    private bool m_bDMand1;

    private bool m_bDMand2;

    private uint m_DMadrs;

    private byte m_DMbval;

    private ushort m_DMwval;

    private bool m_RestartFlag;

    private uint m_DI_cycle;

    private uint m_core_rev_type;

    private readonly INTERRUPTSETTING[] m_InterruptSetting;

    private readonly INTERRUPTAUTO[] m_InterruptAuto;

    private int m_bfrand;

    private uint m_StepOverPC;

    private byte m_MemoryModel;

    private int m_dtrdata;

    private int m_WB;

    private int m_AR;

    private int m_tempreg;

    private int m_mul_div_ex1;

    private int m_mul_div_ex2;

    private int m_borrow;

    private bool m_PcChanged;

    private const int MODE_U8 = 0;

    private const int MODE_U16 = 1;

    public const int SIMU8_ER_API_BUSY = 1;

    public const int SIMU8_ER_ADRS_INVALID = 2;

    public const int SIMU8_ER_ADRS_TOO_LONG = 3;

    public const int SIMU8_ER_NO_SUCH_REGISTER = 4;

    public const int SIMU8_ER_MEMORY_INVALID = 5;

    public const int SIMU8_ER_P_FILE_OPEN_FAILED = 7;

    public const int SIMU8_ER_MUTEX_TIMEOUT = 15;

    public const int SIMU8_ER_SIMU8_FATAL_ERROR = 16;

    public const int SIMU8_ER_INTERRUPT_SIZE_TO_BIG = 17;

    public const int SIMU8_ER_VECTOR_ADRS_INVALID = 18;

    public const int SIMU8_ER_IE_ADRS_INVALID = 19;

    public const int SIMU8_ER_IRQ_ADRS_INVALID = 20;

    public const int SIMU8_ER_IE_BIT_INVALID = 21;

    public const int SIMU8_ER_IRQ_BIT_INVALID = 22;

    public const int SIMU8_ER_SYMBOL_TOO_LONG = 23;

    public const int SIMU8_ER_IOFILE_NOT_EXIST = 24;

    public const int SIMU8_ER_CORE_MODE_INVALID = 25;

    public const int SIMU8_ER_MEM_DLL_TOO_LONG = 26;

    public const int SIMU8_ER_MEM_DLL_NOT_EXIST = 27;

    public const int SIMU8_ER_MEM_WAIT_INVALID = 28;

    public const int SIMU8_ER_DBG_DLL_TOO_LONG = 29;

    public const int SIMU8_ER_DBG_NOT_EXIST = 30;

    public const int SIMU8_ER_COP_DLL_TOO_LONG = 31;

    public const int SIMU8_ER_COP_NOT_EXIST = 32;

    public const int SIMU8_ER_COP_IF_TOO_LONG = 33;

    public const int SIMU8_ER_PERI_DLL_TOO_LONG = 34;

    public const int SIMU8_ER_PERI_NUM_INVALID = 35;

    public const int SIMU8_ER_PERI_ADR_INVALID = 36;

    public const int SIMU8_ER_PERI_INT_NUM_INVALID = 37;

    public const int SIMU8_ER_PERI_INT_VECTOR_INVALID = 38;

    public const int SIMU8_ER_PERI_IE_ADR_INVALID = 39;

    public const int SIMU8_ER_PERI_IE_BIT_INVALID = 40;

    public const int SIMU8_ER_PERI_IRQ_ADR_INVALID = 41;

    public const int SIMU8_ER_PERI_IRQ_BIT_INVALID = 42;

    public const int SIMU8_ER_PERI_SYMBOL_NON = 43;

    public const int SIMU8_ER_STOP_HALT_INFO_ILLEGAL = 44;

    public const int SIMU8_ER_EXCEPTION_ERROR = -1;

    public const int SIMU8_ER_DLL_NOT_EXIST = -2;

    public const int SIMU8_ER_SEG_SIZE_TO_BIG = 8;

    public const int SIMU8_ER_ILLEGAL_RECORD = 9;

    public const int SIMU8_ER_CHECKSUM = 10;

    public const int SIMU8_ER_INVALID_END_RECORD = 11;

    public const int SIMU8_ER_NO_END_RECORD = 12;

    public const int SIMU8_ER_NO_VALID_RECORD = 13;

    public const int SIMU8_ER_ILLEGAL_POS_END_RECORD = 14;

    private const int ERR_NOT_EXIST_DLL = -2;

    private const int ERR_NOT_EXIST_CS8 = 24;

    private const int ERR_INVALID_CORE = 25;

    private const int ERR_LONG_FILENAME_DLL_MEM = 26;

    private const int SIMU8_ER_DLL_NOT_EXIST_MEM = 27;

    private const int ERR_INVALID_MEMWAIT = 28;

    private const int ERR_LONG_FILENAME_DLL_DEBUG = 29;

    private const int SIMU8_ER_DLL_NOT_EXIST_DEBUG = 30;

    private const int ERR_LONG_FILENAME_DLL_COPRO = 31;

    private const int SIMU8_ER_DLL_NOT_EXIST_COPRO = 32;

    private const int ERR_INVALID_COPROID = 33;

    private const int ERR_LONG_FILENAME_DLL_PERIPHERAL = 34;

    private const int ERR_INVALID_PERIPHERAL_NUM = 35;

    private const int ERR_INVALID_START_ADRS = 36;

    private const int ERR_INVALID_INT_NUM = 37;

    private const int ERR_INVALID_INT_VECTOR = 38;

    private const int ERR_INVALID_IE_ADRS = 39;

    private const int ERR_INVALID_IE_BIT = 40;

    private const int ERR_INVALID_IRQ_ADRS = 41;

    private const int ERR_INVALID_IRQ_BIT = 42;

    private const int ERR_INVALID_SYM_NAME = 43;

    private const int ERR_INVALID_STP_ADRS = 44;

    private string m_trg_name;

    private int m_cpu_mode;

    private uint m_cpu_series;

    private uint m_cs8_version;

    private readonly uint m_pc;

    private uint m_csr;

    private uint m_pc20;

    private uint m_pcbak;

    private uint m_next_pc;

    private uint m_next_next_pc;

    private uint m_execute_pc;

    private readonly uint m_dsr;

    private int m_inst_mode;

    private readonly bool m_mselect;

    private bool m_reset_req;

    private bool m_nmice_req;

    private bool m_nmi_req;

    private bool m_mi_req;

    private bool m_swi_req;

    private ushort m_vector;

    private readonly int m_intnum;

    private int m_current_irqno;

    private bool m_int_cycle_id;

    private bool m_int_cycle_ex;

    private bool m_dsr_prifix_inst_ex;

    private bool m_bcctrue;

    private bool m_int_enable;

    private byte m_excom_out;

    private ushort m_pcrdata;

    private ushort m_IR;

    private ushort m_saveIR;

    private byte m_int_level;

    private bool m_inst_DI;

    private bool m_ext_dtwait;

    private bool m_ext_pcwait;

    private bool m_ext_rwinsel;

    private int m_rwin_wait_counter;

    private bool EX_wait_req;

    private bool ID_wait_req;

    private bool IF_wait_req;

    private bool m_eawait_id;

    private readonly bool m_pipe_hzd;

    private int m_ex_state;

    private int m_id_state;

    private bool m_load_mode;

    private bool m_byte_mode;

    private bool m_word_mode;

    private bool m_dword_mode;

    private bool m_eaplus_mode;

    private bool m_dstart_ex;

    private bool m_dend_ex;

    private bool m_pc_wait_ex;

    private bool m_edsr_ex;

    private bool m_ea_plus_ex;

    private bool m_edsr_ex_used;

    private bool m_dstart_id;

    private bool m_dend_id;

    private bool m_dtword_id;

    private bool m_dtsize_id;

    private bool m_bcc_id;

    private bool m_brk_id;

    private bool m_brkswi_id;

    private bool m_clear_temp;

    private bool m_cntl_axxx_id;

    private bool m_cop_read_id;

    private bool m_cop_store_id;

    private bool m_data_move_id;

    private bool m_disint_user_id;

    private bool m_dtlock_id;

    private bool m_ea_mode_id;

    private bool m_ea_plus_id;

    private bool m_exe_alu_id;

    private bool m_exe_div_id;

    private bool m_exe_mul_div_wb_id;

    private bool m_exe_mul_div_wr_c_id;

    private bool m_exe_mul_id;

    private bool m_exe_rn_rm_id;

    private bool m_exe_word1_id;

    private bool m_exe_word3_id;

    private bool m_exe_wr_z_id;

    private bool m_exe_wr_zand_id;

    private bool m_exebit_id;

    private bool m_extend_id;

    private bool m_greg0_entry_id;

    private bool m_greg1_entry_id;

    private bool m_greg2_entry_id;

    private bool m_incdecea_id;

    private bool m_intack_id;

    private bool m_ir_bit10_clr;

    private bool m_ir_bit11_clr;

    private bool m_ir_bit8_clr;

    private bool m_ir_bit9_clr;

    private bool m_lea_dirct_id;

    private bool m_left_id;

    private bool m_memory_load_ea_id;

    private bool m_memory_load_id;

    private bool m_memory_pop_id;

    private bool m_memory_read_id;

    private bool m_memory_store_id;

    private bool m_neg_id;

    private bool m_pc_clear_id;

    private bool m_pc_wait_id;

    private bool m_pcl_wait_id;

    private bool m_pcstb_id;

    private bool m_pop_eah_id;

    private bool m_pop_eal_id;

    private bool m_pswbit_id;

    private bool m_resend_id;

    private bool m_right_id;

    private bool m_s1_swi_id;

    private bool m_sel_a16l_eabus_id;

    private bool m_sel_a16l_pc_id;

    private bool m_sel_a16r_1_id;

    private bool m_sel_a16r_abus_id;

    private bool m_sel_a16r_ff_id;

    private bool m_sel_abus_bound_id;

    private bool m_sel_abus_swap_id;

    private bool m_sel_abus_width_id;

    private bool m_sel_adbus_ea_id;

    private bool m_sel_arbus_eabus_id;

    private bool m_sel_cbus1_bcc_id;

    private bool m_sel_cbus1_borrow_id;

    private bool m_sel_cbus1_clrl_id;

    private bool m_sel_cbus1_const1_id;

    private bool m_sel_cbus1_daa_id;

    private bool m_sel_cbus1_ecsr_id;

    private bool m_sel_cbus1_elrh_id;

    private bool m_sel_cbus1_elrl_id;

    private bool m_sel_cbus1_epsw_id;

    private bool m_sel_cbus1_irl_id;

    private bool m_sel_cbus1_mul_div_ex2_id;

    private bool m_sel_cbus1_psw_id;

    private bool m_sel_cbus1_roml_id;

    private bool m_sel_cbus1_sph_id;

    private bool m_sel_cbus1_spl_id;

    private bool m_sel_cbus1_wbdata_id;

    private bool m_sel_cbus2_clrh_id;

    private bool m_sel_cbus2_elrh_id;

    private bool m_sel_cbus2_irl_id;

    private bool m_sel_cbus2_romh_id;

    private bool m_sel_cbus2_wbdata_id;

    private bool m_sel_csr_cbus0_id;

    private bool m_sel_csr_clr_id;

    private bool m_sel_csr_irn_id;

    private bool m_sel_eabus_sp_id;

    private bool m_sel_ex1_cbus0_id;

    private bool m_sel_ex2_cbus1_id;

    private bool m_sel_excom_irh;

    private bool m_sel_excom_irl;

    private bool m_sel_excom_irm;

    private bool m_sel_excom_irn;

    private bool m_sel_greg0_bp_id;

    private bool m_sel_greg0_fp_id;

    private bool m_sel_greg0_regn_bit0or_id;

    private bool m_sel_greg0_regn_bit1or_id;

    private bool m_sel_greg0_regn_bit2or_id;

    private bool m_sel_greg1_regm_bit0or_id;

    private bool m_sel_pc_pcbus_id;

    private bool m_sel_pc_swivec_id;

    private bool m_sel_psw_mi_id;

    private bool m_sel_psw_nmi_id;

    private bool m_sel_psw_nmice_id;

    private bool m_sel_r0_clrh;

    private bool m_sel_r0_clrl;

    private bool m_sel_r0_eah;

    private bool m_sel_r0_eal;

    private bool m_sel_r0_ecsr;

    private bool m_sel_r0_elrh;

    private bool m_sel_r0_elrl;

    private bool m_sel_r0_epsw;

    private bool m_sel_r0_irl;

    private bool m_sel_r0_lcsr;

    private bool m_sel_r0_mul_div;

    private bool m_sel_r0_psw;

    private bool m_sel_temp_id;

    private bool m_shift_id;

    private bool m_state_clr_id;

    private bool m_tbl_ffef_id;

    private bool m_wr_arh_id;

    private bool m_wr_arl_id;

    private bool m_wr_csr_wb_id;

    private bool m_wr_disp6_id;

    private bool m_wr_eah_id;

    private bool m_wr_eal_id;

    private bool m_wr_ecsr_csr_id;

    private bool m_wr_elr_pc_id;

    private bool m_wr_elrl_wb_id;

    private bool m_wr_ex1_wb_id;

    private bool m_wr_intecsr_csr_id;

    private bool m_wr_intelr_pc_id;

    private bool m_wr_intecsr_oldcsr_id;

    private bool m_wr_intelr_oldpc_id;

    private bool m_wr_iceswi_id;

    private bool m_wr_lcsr_csr_id;

    private bool m_wr_lcsr_wb_id;

    private bool m_wr_lr_nextpc_id;

    private bool m_wr_lrl_wb_id;

    private bool m_wr_pcl_wb_id;

    private bool m_wr_psw_epsw_id;

    private bool m_wr_psw_wb_id;

    private bool m_wr_sp_id;

    private bool m_edsr_id;

    private bool m_disint_id;

    private bool m_cntl_move_id;

    private bool m_wr_lr_pc_id;

    private bool m_iceswi_id;

    private bool m_wr_greg_wb_id;

    private bool m_alu_cpc_flag;

    private bool m_alu_add_flag;

    private bool m_alu_adc_flag;

    private bool m_alu_sub_flag;

    private bool m_alu_sbc_flag;

    private bool m_alu_mov_flag;

    private bool m_alu_and_flag;

    private bool m_alu_or_flag;

    private bool m_alu_xor_flag;

    private bool m_alu_rb_flag;

    private bool m_alu_sb_flag;

    private bool m_alu_daa_flag;

    private bool m_alu_das_flag;

    private bool m_alu_mul_flag;

    private bool m_alu_div_flag;

    private bool m_alu_cmp_flag;

    private bool m_alu_reverse_flag;

    private bool m_alu_inc_flag;

    private bool m_alu_dec_flag;

    private bool m_alu_adsp_flag;

    private bool m_shift_imm7_flag;

    private bool m_shift_signextend_flag;

    private bool m_shift_extend_flag;

    private bool m_shift_sra_flag;

    private bool m_shift_srlc_flag;

    private bool m_shift_sllc_flag;

    private bool m_shift_sll_flag;

    private bool m_shift_srl_flag;

    private bool m_step_inst1_flag;

    private bool m_step_inst2_flag;

    private bool m_alu_psw_or_flag;

    private bool m_alu_psw_and_flag;

    private string DllCopro_name;

    private string DllMem_name;

    private string DllDbg_name;

    public dynamic CSimCoproApp;

    public dynamic CSimDbgApp;

    public dynamic CSimMemApp;

    public dynamic periApp;

    private uint DllCopro_id;

    private uint DllNum_SimPeri;

    private readonly SimPeriInfo[] DllPeri_info;

    private int DllMem_wait;

    public dynamic[] hSimPeripheralDLLInst;

    public static byte[] ICESWI_CODE()
    {
        return new byte[2] { 255, 254 };
    }

    public byte m_GetDSR()
    {
        return m_DSR;
    }

    public void m_SetDsrPrefix(byte flag)
    {
        m_DsrPrefixFlag = flag;
    }

    public byte m_GetDsrPrefix()
    {
        return m_DsrPrefixFlag;
    }

    public byte GetSimRun()
    {
        return m_SimRunFlag;
    }

    public CSimU8main()
    {
        m_InterruptInfo = new INTERRUPTTABLE[64];
        TrcRamAdr = new uint[16];
        RdRamAdr = new uint[16];
        TrcRamDat = new byte[16];
        RdRamDat = new byte[16];
        m_InterruptSetting = new INTERRUPTSETTING[64];
        m_InterruptAuto = new INTERRUPTAUTO[64];
        DllPeri_info = new SimPeriInfo[32];
        SimPeriInfo[] dllPeri_info = DllPeri_info;
        for (int i = 0; i < dllPeri_info.Length; i++)
        {
            ((SimPeriInfo)dllPeri_info[i]).InitIntInfo();
        }
        m_SimRunFlag = 7;
        m_Initflag = false;
        m_DefaultCode = -1;
        m_gcount = 0u;
        m_Logflag = 0;
        m_CodeStartAdr = 1u;
        m_CodeEndAdr = 0u;
        m_DataStartAdr = 1u;
        m_DataEndAdr = 0u;
        m_RomWinStartAdr = 1u;
        m_RomWinEndAdr = 0u;
        m_SetDsrPrefix(0);
        m_STPACPflag = 0;
        m_StepFlag = 0;
        m_TraceLastMemAdr = 0u;
        m_TraceLastMemDat.data = 0u;
        m_TraceLastMemDat.byte_h_l = 0;
        m_CvCntAddressStart = 0u;
        m_CvCntAddressEnd = 65535u;
        m_DSR = 0;
        m_NextPC = 0u;
        m_BreakPC = 0u;
        m_TraceNum = 0u;
        m_MemoryModel = 1;
        m_StepInst = 0;
        m_StepOverPC = 0u;
        m_bInstEAplus = false;
        for (int j = 0; j < 8; j++)
        {
            TrcRamAdr[j] = 0u;
            TrcRamDat[j] = 0;
            RdRamAdr[j] = 0u;
            RdRamDat[j] = 0;
        }
        TrcRamCnt = 0;
        RdRamCnt = 0;
        m_bDMand1 = false;
        m_bDMand2 = false;
        m_RestartFlag = false;
        m_beforeCycleCounter = 0uL;
        m_bfrand = LIBC.time();
        m_TraceBreakPointer = 0u;
        m_pc = 0u;
        m_csr = 0u;
        m_dsr = 0u;
        m_pc20 = 0u;
        m_pcbak = 0u;
        m_next_pc = 0u;
        m_next_next_pc = 0u;
        m_execute_pc = 0u;
        m_inst_mode = 0;
        m_mselect = false;
        m_excom_out = 0;
        m_IR = 0;
        m_saveIR = 0;
        m_ex_state = 0;
        m_id_state = 0;
        m_PcChanged = false;
        m_CycleCountEnable = 0;
        m_reset_req = false;
        m_nmice_req = false;
        m_nmi_req = false;
        m_mi_req = false;
        m_swi_req = false;
        m_vector = 0;
        m_int_enable = false;
        m_intnum = 0;
        m_bcctrue = false;
        m_int_level = 0;
        m_IdIsEmpty = true;
        m_IfIsEmpty = true;
        m_ext_dtwait = false;
        m_ext_pcwait = false;
        m_ext_rwinsel = false;
        EX_wait_req = false;
        ID_wait_req = false;
        IF_wait_req = false;
        m_eawait_id = false;
        m_pipe_hzd = false;
        m_rwin_wait_counter = 0;
        m_ClearIdSetFlg();
        m_ClearExSetFlg();
        m_edsr_ex = false;
        m_edsr_ex_used = false;
        m_dsr_prifix_inst_ex = false;
        m_initPeriData();
        m_readIOfile();
        m_DynamicLink();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    ~CSimU8main()
    {
        Dispose(disposing: false);
    }

    public int m_SetCodeMemorySize(uint startadrs, uint endadrs)
    {
        int num = 0;
        uint num2 = 0u;
        uint num3 = 0u;
        if (startadrs > 1048575)
        {
            num = 5;
        }
        if (endadrs > 1048575)
        {
            num = 5;
        }
        if (startadrs > endadrs)
        {
            num = 2;
        }
        if (num != 0)
        {
            CSimMemApp.memApi_SetRange(12289, 0, 0);
            CSimMemApp.memApi_SetRange(12321, 0, 0);
            CSimMemApp.memApi_SetRange(12305, 0, 0);
            CSimDbgApp.dbgApi_SetIERange(0, 0);
            m_CodeStartAdr = 0u;
            m_CodeEndAdr = 0u;
            m_DataStartAdr = 0u;
            m_DataEndAdr = 0u;
        }
        else
        {
            if (startadrs % 2 == 1)
            {
                startadrs--;
            }
            if (endadrs % 2 == 0)
            {
                endadrs++;
            }
            if (endadrs < 65535)
            {
                endadrs = 65535u;
            }
            CSimMemApp.memApi_GetRange(12321, ref num2, ref num3);
            if (endadrs < 65536)
            {
                CSimMemApp.memApi_SetRange(12289, startadrs, endadrs);
                if (num2 > num3)
                {
                    CSimMemApp.memApi_SetRange(12321, 0, 0);
                }
                else if (m_CodeEndAdr > m_DataEndAdr)
                {
                    if (m_DataEndAdr < 65536)
                    {
                        m_DataEndAdr = 65536u;
                    }
                    CSimMemApp.memApi_SetRange(12321, 0, m_DataEndAdr - 65536);
                }
            }
            else
            {
                CSimMemApp.memApi_SetRange(12289, startadrs, 65535);
                if (m_DataEndAdr < endadrs)
                {
                    CSimMemApp.memApi_SetRange(12321, 0, endadrs - 65536);
                }
                else if (m_CodeEndAdr > m_DataEndAdr)
                {
                    if (m_DataEndAdr < 65536)
                    {
                        m_DataEndAdr = 65536u;
                    }
                    CSimMemApp.memApi_SetRange(12321, 0, m_DataEndAdr - 65536);
                }
            }
            CSimDbgApp.dbgApi_SetIERange(startadrs, endadrs);
            m_CodeStartAdr = startadrs;
            m_CodeEndAdr = endadrs;
        }
        if (m_Logflag == 1)
        {
            new COutMod().W(" Result:").endl();
            new COutMod().W(" Code Range: ").hex(m_CodeStartAdr).W(" - ")
                .hex(m_CodeEndAdr)
                .endl();
            CSimMemApp.memApi_GetRange(12289, ref num2, ref num3);
            new COutMod().W(" CMem Start: ").hex(num2).W("  End: ")
                .hex(num3)
                .endl();
            CSimMemApp.memApi_GetRange(12321, ref num2, ref num3);
            new COutMod().W(" GMem Start: ").hex(num2).W("  End: ")
                .hex(num3)
                .endl();
        }
        return num;
    }

    public int m_SetDataMemorySize(uint startadrs, uint endadrs)
    {
        int num = 0;
        uint num2 = 0u;
        uint num3 = 0u;
        if (startadrs > 16777215)
        {
            num = 5;
        }
        if (endadrs > 16777215)
        {
            num = 5;
        }
        if (startadrs > endadrs)
        {
            num = 2;
        }
        if (num != 0)
        {
            CSimMemApp.memApi_SetRange(12305, 0, 0);
            CSimMemApp.memApi_SetRange(12321, 0, 0);
            CSimMemApp.memApi_SetRange(12289, 0, 0);
            m_DataStartAdr = 0u;
            m_DataEndAdr = 0u;
            m_CodeStartAdr = 0u;
            m_CodeEndAdr = 0u;
        }
        else
        {
            if (startadrs % 2 == 1)
            {
                startadrs--;
            }
            if (endadrs % 2 == 0)
            {
                endadrs++;
            }
            if (endadrs < 65535)
            {
                endadrs = 65535u;
            }
            CSimMemApp.memApi_GetRange(12321, ref num2, ref num3);
            if (endadrs < 65536)
            {
                CSimMemApp.memApi_SetRange(12305, startadrs, endadrs);
                if (num2 > num3)
                {
                    CSimMemApp.memApi_SetRange(12321, 0, 0);
                }
                else if (m_CodeEndAdr < m_DataEndAdr)
                {
                    if (m_CodeEndAdr < 65536)
                    {
                        m_CodeEndAdr = 65536u;
                    }
                    CSimMemApp.memApi_SetRange(12321, 0, m_CodeEndAdr - 65536);
                }
            }
            else
            {
                CSimMemApp.memApi_SetRange(12305, startadrs, 65535);
                if (m_CodeEndAdr < endadrs)
                {
                    CSimMemApp.memApi_SetRange(12321, 0, endadrs - 65536);
                }
                else if (m_CodeEndAdr < m_DataEndAdr)
                {
                    if (m_CodeEndAdr < 65536)
                    {
                        m_CodeEndAdr = 65536u;
                    }
                    CSimMemApp.memApi_SetRange(12321, 0, m_CodeEndAdr - 65536);
                }
            }
            m_DataStartAdr = startadrs;
            m_DataEndAdr = endadrs;
        }
        if (m_Logflag == 1)
        {
            new COutMod().W(" Result:").endl();
            new COutMod().W(" Data Range: ").hex(m_DataStartAdr).W(" - ")
                .hex(m_DataEndAdr)
                .endl();
            CSimMemApp.memApi_GetRange(12305, ref num2, ref num3);
            new COutMod().W(" DMem Start: ").hex(num2).W("  End: ")
                .hex(num3)
                .endl();
            CSimMemApp.memApi_GetRange(12321, ref num2, ref num3);
            new COutMod().W(" GMem Start: ").hex(num2).W("  End: ")
                .hex(num3)
                .endl();
        }
        return num;
    }

    public int m_SetRomWindowSize(uint startadrs, uint endadrs)
    {
        int num = 0;
        if (startadrs > 65535)
        {
            num = 2;
        }
        if (endadrs > 65535)
        {
            num = 2;
        }
        if (startadrs > endadrs)
        {
            num = 2;
        }
        if (num != 0 || (startadrs == 0 && endadrs == 0))
        {
            m_RomWinStartAdr = 0u;
            m_RomWinEndAdr = 0u;
        }
        else
        {
            if (startadrs % 2 == 1)
            {
                startadrs--;
            }
            if (endadrs % 2 == 0)
            {
                endadrs++;
            }
            m_RomWinStartAdr = startadrs;
            m_RomWinEndAdr = endadrs;
        }
        if (m_Logflag == 1)
        {
            new COutMod().W(" Result:").endl();
            new COutMod().W(" RomWindow Range: ").hex(m_RomWinStartAdr).W(" - ")
                .hex(m_RomWinEndAdr)
                .endl();
        }
        return num;
    }

    public int m_SetCodeMemoryDefaultCode(ushort val)
    {
        uint num = 0u;
        m_DefaultCode = val;
        uint num2 = 0u;
        uint num3 = 0u;
        uint num4 = 0u;
        uint num5 = 0u;
        CSimMemApp.memApi_GetRange(12289, ref num2, ref num3);
        CSimMemApp.memApi_GetRange(12321, ref num4, ref num5);
        if (num2 > num3)
        {
            num2 = 0u;
            num3 = 65535u;
            num4 = 0u;
            num5 = 983039u;
            CSimMemApp.memApi_SetRange(12289, num2, num3);
            CSimMemApp.memApi_SetRange(12321, num4, num5);
            m_CodeStartAdr = 0u;
            m_CodeEndAdr = 1048575u;
        }
        byte b = BM.I2B(val & 0xFF);
        byte b2 = BM.I2B((val >> 8) & 0xFF);
        for (num = num2; num < num3; num += 2)
        {
            CSimMemApp.memApi_SetVal(12289, num, b);
            CSimMemApp.memApi_SetVal(12289, num + 1, b2);
        }
        if (num4 < num5)
        {
            for (num = num4; num < num5; num += 2)
            {
                CSimMemApp.memApi_SetVal(12321, num, b);
                CSimMemApp.memApi_SetVal(12321, num + 1, b2);
            }
        }
        return 0;
    }

    public int m_LoadHexFile(string filename)
    {
        if (!m_Initflag)
        {
            InitSetting();
        }
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string inputFile;
        if (baseDirectory.StartsWith("/", StringComparison.Ordinal))
        {
            LIBC.AdjustMacPath(baseDirectory, out var cpath);
            inputFile = string.Format("{0}{1}{2}", cpath, "Resources/", filename);
        }
        else
        {
            inputFile = $"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[1] { '\\' })}\\{filename}";
        }
        return AnalyzeInputHexFile(inputFile);
    }

    public int m_SimReset(int run_mode)
    {
        int num = 0;
        ushort val = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        m_Reg.Reset();
        if (num == 0)
        {
            CSimMemApp.memApi_GetWordVal(12289, 0, ref val);
            m_WriteReg(30, val);
            CSimMemApp.memApi_GetWordVal(12289, 2, ref val);
            m_WriteReg(16, val);
            m_IdIsEmpty = true;
            m_IfIsEmpty = true;
            m_id_state = 0;
            m_ex_state = 0;
        }
        m_SetDsrPrefix(0);
        m_STPACPflag = 0;
        if (run_mode != 1)
        {
            m_SimRunFlag = 7;
            if (num != -2)
            {
                CSimDbgApp.dbgApi_SetCycleCounter(0);
            }
            m_InitAutoInterruptCounter();
            InitSfr();
            if (num != -2)
            {
                CSimDbgApp.dbgApi_SetBreakStatus((ushort)0);
            }
            uint num2 = 0u;
            CSimDbgApp.dbgApi_GetTraceCountBP(ref num2);
            CSimDbgApp.dbgApi_SetTraceCountBP(num2);
        }
        else if (m_SimRunFlag == 7 || m_SimRunFlag == 1 || m_SimRunFlag == 2)
        {
            m_SimRunFlag = 0;
        }
        else
        {
            m_SimRunFlag = 2;
        }
        m_ExternalReset();
        m_ClearIdSetFlg();
        m_id_state = 0;
        m_ex_state = 0;
        m_ext_dtwait = false;
        m_ext_pcwait = false;
        m_ext_rwinsel = false;
        EX_wait_req = false;
        ID_wait_req = false;
        IF_wait_req = false;
        m_eawait_id = false;
        m_pcbak = 0u;
        m_pc20 = 0u;
        m_execute_pc = 0u;
        m_TraceLastMemAdr = 0u;
        m_TraceLastMemDat.byte_h_l = 0;
        m_TraceLastMemDat.data = 0u;
        return num;
    }

    public int m_InitAutoInterruptCounter()
    {
        for (int i = 0; i < m_InterruptNum; i++)
        {
            m_InterruptAuto[i].onceFlag = false;
            if (m_InterruptSetting[i].autoInterrupt)
            {
                if (m_InterruptSetting[i].onceOption && m_InterruptSetting[i].onceMode == 1)
                {
                    m_InterruptAuto[i].onceCount = m_InterruptSetting[i].onceCycle;
                }
                if (m_InterruptSetting[i].repeatOption)
                {
                    SetAutoIntCount(i);
                }
            }
        }
        return 0;
    }

    public int m_SimStop()
    {
        int num = 0;
        uint num2 = 0u;
        if (!m_Initflag)
        {
            InitSetting();
        }
        m_SimRunFlag = 16;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
            num2 |= 0x80u;
            CSimDbgApp.dbgApi_SetBreakStatus(num2);
        }
        return num;
    }

    public int m_WriteCodeMemory(uint adrs, uint len, byte[] val)
    {
        int num = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        uint num2 = (uint)((int)adrs + ((len > val.Length) ? val.Length : ((int)len)) - 1);
        if (adrs < 65536)
        {
            uint num3 = ((num2 >= 65536) ? 65535u : num2);
            for (uint num4 = adrs; num4 <= num3; num4++)
            {
                if (num4 < m_CodeStartAdr || num4 > m_CodeEndAdr)
                {
                    return 2;
                }
                byte b = val[num];
                if ((int)CSimMemApp.memApi_SetVal(12289, num4, b) == -1)
                {
                    return 2;
                }
                num++;
            }
            if (m_Logflag == 1)
            {
                byte v = 0;
                adrs = ((adrs > 16) ? (adrs - 16) : adrs);
                num3 = ((num3 < 65520) ? (num3 + 16) : num3);
                COutMod cOutMod = new COutMod();
                for (uint num4 = adrs; num4 <= num3; num4++)
                {
                    CSimMemApp.memApi_GetVal(12289, num4, ref v);
                    cOutMod.hex2dig(v).W(" ");
                    if ((num4 & 0xF) == 15)
                    {
                        cOutMod.endl();
                    }
                }
                cOutMod.endl();
            }
        }
        if (num2 >= 65536)
        {
            adrs = ((adrs >= 65536) ? (adrs - 65536) : 0u);
            num2 -= 65536;
            uint num5 = ((m_CodeStartAdr >= 65536) ? (m_CodeStartAdr - 65536) : 0u);
            uint num6 = ((m_CodeEndAdr >= 65536) ? (m_CodeEndAdr - 65536) : 0u);
            for (uint num4 = adrs; num4 <= num2; num4++)
            {
                if (num4 < num5 || num4 > num6)
                {
                    return 2;
                }
                byte b = val[num];
                if ((int)CSimMemApp.memApi_SetVal(12321, num4, b) == -1)
                {
                    return 2;
                }
                num++;
            }
            if (m_Logflag == 1)
            {
                byte v2 = 0;
                adrs = ((adrs > 16) ? (adrs - 16) : adrs);
                num2 = ((num2 < 983024) ? (num2 + 16) : num2);
                COutMod cOutMod2 = new COutMod();
                for (uint num4 = adrs; num4 <= num2; num4++)
                {
                    CSimMemApp.memApi_GetVal(12321, num4, ref v2);
                    cOutMod2.hex2dig(v2).W(" ");
                    if ((num4 & 0xF) == 15)
                    {
                        cOutMod2.endl();
                    }
                }
                cOutMod2.endl();
            }
        }
        return 0;
    }

    public int m_ReadDataMemory(uint adrs, uint len, byte[] val)
    {
        int num = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        uint num2 = adrs + len - 1;
        if (adrs < 65536)
        {
            uint num3 = ((num2 >= 65536) ? 65535u : num2);
            for (uint num4 = adrs; num4 <= num3; num4++)
            {
                if (num4 < m_DataStartAdr || num4 > m_DataEndAdr)
                {
                    return 2;
                }
                int num5 = ReadDMemory(num4, val, num);
                if (num5 != 0)
                {
                    return num5;
                }
                num++;
            }
        }
        if (num2 >= 65536)
        {
            if (m_DataEndAdr < 65536)
            {
                return 2;
            }
            adrs = ((adrs >= 65536) ? (adrs - 65536) : 0u);
            num2 -= 65536;
            uint num6 = ((m_DataStartAdr >= 65536) ? (m_DataStartAdr - 65536) : 0u);
            uint num7 = m_DataEndAdr - 65536;
            for (uint num4 = adrs; num4 <= num2; num4++)
            {
                if (num4 < num6 || num4 > num7)
                {
                    return 2;
                }
                byte b = 0;
                if ((int)CSimMemApp.memApi_GetVal(12321, num4, ref b) == -1)
                {
                    return 2;
                }
                val[num] = b;
                num++;
            }
        }
        return 0;
    }

    public int m_WriteDataMemory(uint adrs, uint len, byte[] val)
    {
        int num = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        uint num2 = adrs + len - 1;
        if (adrs < 65536)
        {
            uint num3 = ((num2 >= 65536) ? 65535u : num2);
            for (uint num4 = adrs; num4 <= num3; num4++)
            {
                if (num4 < m_DataStartAdr || num4 > m_DataEndAdr)
                {
                    return 2;
                }
                int num5 = WriteDMemory(num4, val[num]);
                if (num5 != 0)
                {
                    return num5;
                }
                num++;
            }
            COutMod cOutMod = new COutMod();
            if (m_Logflag == 1)
            {
                byte[] array = new byte[1];
                adrs = ((adrs > 16) ? (adrs - 16) : adrs);
                num3 = ((num3 < 65520) ? (num3 + 16) : num3);
                for (uint num4 = adrs; num4 <= num3; num4++)
                {
                    ReadDMemory(num4, array, 0);
                    cOutMod.hex2dig(array[0]).W(" ");
                    if ((num4 & 0xF) == 15)
                    {
                        cOutMod.endl();
                    }
                }
                cOutMod.endl();
            }
        }
        if (num2 >= 65536)
        {
            if (m_DataEndAdr < 65536)
            {
                return 2;
            }
            adrs = ((adrs >= 65536) ? (adrs - 65536) : 0u);
            num2 -= 65536;
            uint num6 = ((m_DataStartAdr >= 65536) ? (m_DataStartAdr - 65536) : 0u);
            uint num7 = m_DataEndAdr - 65536;
            for (uint num4 = adrs; num4 <= num2; num4++)
            {
                if (num4 < num6 || num4 > num7)
                {
                    return 2;
                }
                if ((int)CSimMemApp.memApi_SetVal(12321, num4, val[num]) == -1)
                {
                    return 2;
                }
                num++;
            }
            COutMod cOutMod2 = new COutMod();
            if (m_Logflag == 1)
            {
                byte v = 0;
                adrs = ((adrs > 16) ? (adrs - 16) : adrs);
                num2 = ((num2 < 16711664) ? (num2 + 16) : num2);
                for (uint num4 = adrs; num4 <= num2; num4++)
                {
                    CSimMemApp.memApi_GetVal(12321, num4, ref v);
                    cOutMod2.hex2dig(v).W(" ");
                    if ((num4 & 0xF) == 15)
                    {
                        cOutMod2.endl();
                    }
                }
                cOutMod2.endl();
            }
        }
        return 0;
    }

    public int m_WriteBitDataMemory(uint adrs, byte n, byte val)
    {
        byte[] array = new byte[1];
        if (!m_Initflag)
        {
            InitSetting();
        }
        switch (m_ReadDataMemory(adrs, 1u, array))
        {
            case -2:
                return -2;
            default:
                return 2;
            case 0:
                {
                    byte b = BM.I2B(1 << (n & 7));
                    if ((val & 1) == 1)
                    {
                        array[0] |= b;
                    }
                    else
                    {
                        array[0] &= BM.I2B(~b);
                    }
                    int num = m_WriteDataMemory(adrs, 1u, array);
                    if (num == -2)
                    {
                        return -2;
                    }
                    if (m_Logflag == 2)
                    {
                        string line = $"----- WriteBitDataMemory : {adrs:04X}.{n} <- {val} -----\n";
                        try
                        {
                            f1.WriteString(line);
                        }
                        catch (IOException)
                        {
                            f1.Abort();
                            exit(1);
                        }
                    }
                    return num;
                }
        }
    }

    public int m_ReadReg(byte regtype, ref uint val)
    {
        int result = 0;
        int num = 0;
        if (32 <= regtype && regtype <= 47)
        {
            if ((object)CSimCoproApp == null)
            {
                result = -1;
            }
            else
            {
                result = CSimCoproApp.cpApi_GetReg(BM.I2B(regtype - 32), ref val);
                if (!m_Initflag)
                {
                    InitSetting();
                }
            }
        }
        else
        {
            num = m_Reg.GetReg(regtype);
            if (!m_Initflag)
            {
                InitSetting();
            }
            if (num == -1)
            {
                result = 4;
            }
            else
            {
                val = (uint)num;
            }
        }
        return result;
    }

    public int m_WriteReg(byte regtype, uint val)
    {
        if (!m_Initflag)
        {
            InitSetting();
        }
        int num;
        if (32 <= regtype && regtype <= 47)
        {
            num = (((object)CSimCoproApp != null) ? ((int)CSimCoproApp.cpApi_SetReg(BM.I2B(regtype - 32), val)) : (-1));
        }
        else
        {
            num = m_Reg.SetReg(regtype, val);
            if (num == -1)
            {
                num = 4;
            }
        }
        return num;
    }

    public int m_LogState()
    {
        return m_Logflag;
    }

    public int m_LogStart()
    {
        m_Logflag = 1;
        return 0;
    }

    public int m_LogStart2(string fname)
    {
        string text;
        if (!f1.OpenForCreate(fname))
        {
            text = $"ファイル {fname} をオープンできません。\n実行を終了します。";
            exit(1);
        }
        text = "       PC    Code      PSW R0  R1  R2  R3  R4  R5  R6  R7  R8  R9  R10 R11 R12 R13 R14 R15 EA   SP   LR\n" + "===============================================================================================================\n";
        try
        {
            f1.WriteString(text);
        }
        catch (IOException)
        {
            f1.Abort();
            exit(1);
        }
        m_Logflag = 2;
        return 0;
    }

    public int m_LogStop()
    {
        if (m_Logflag == 2)
        {
            f1.Close();
        }
        m_Logflag = 0;
        return 0;
    }

    public int m_ReadCodeMemory(uint adrs, uint len, byte[] val)
    {
        int num = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        uint num2 = adrs + len - 1;
        if (adrs < 65536)
        {
            uint num3 = ((num2 >= 65536) ? 65535u : num2);
            for (uint num4 = adrs; num4 <= num3; num4++)
            {
                byte b = 0;
                int num5 = CSimMemApp.memApi_GetVal(12289, num4, ref b);
                val[num] = b;
                if (num5 != 0)
                {
                    return num5;
                }
                num++;
            }
        }
        if (num2 >= 65536)
        {
            adrs = ((adrs >= 65536) ? (adrs - 65536) : 0u);
            num2 -= 65536;
            for (uint num4 = adrs; num4 <= num2; num4++)
            {
                byte b2 = 0;
                if ((int)CSimMemApp.memApi_GetVal(12321, num4, ref b2) == -1)
                {
                    return 2;
                }
                val[num] = b2;
                num++;
            }
        }
        return 0;
    }

    public int m_GetCount(ref uint cnt)
    {
        cnt = m_gcount;
        return 0;
    }

    public int AnalyzeInputHexFile(string inputFile)
    {
        ulong maxAddr = 0uL;
        ulong minAddr = 4294967295uL;
        int num = InputFileClassify(inputFile, ref maxAddr, ref minAddr);
        if (num == 0 && maxAddr > 1048575)
        {
            num = 8;
        }
        if (num == 0)
        {
            num = AnalyzeIntelHex(inputFile);
        }
        return num;
    }

    public int AnalyzeIntelHex(string inputFile)
    {
        int num = 0;
        CStdioFileR cStdioFileR = new CStdioFileR();
        if (cStdioFileR.OpenForRead(inputFile))
        {
            if (cStdioFileR.GetLength() == 0)
            {
                return 13;
            }
            uint num2 = 0u;
            uint num3 = 0u;
            bool flag = true;
            string line = null;
            bool flag2 = false;
            while (cStdioFileR.ReadString(ref line) && flag)
            {
                if (string.IsNullOrEmpty(line) || line[0] != ':')
                {
                    continue;
                }
                for (int i = 1; i < line.Length; i++)
                {
                    if (!isxdigit(line[i]))
                    {
                        flag = false;
                        return 9;
                    }
                }
                if (!CheckIntelHexCheckSum(line))
                {
                    flag = false;
                    return 10;
                }
                if (flag2)
                {
                    flag = false;
                    return 14;
                }
                byte b = StrToByte(line, 1);
                uint num4 = StrToWord(line, 3);
                byte b2 = StrToByte(line, 7);
                if (b != (line.Length - 1 - 2 - 4 - 2 - 2) / 2)
                {
                    flag = false;
                    return 9;
                }
                switch (b2)
                {
                    case 1:
                        flag2 = true;
                        if (!string.Equals(line, ":00000001FF", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = false;
                            num = 11;
                        }
                        break;
                    case 2:
                        num2 = BM.I2UI((StrToWord(line, 9) << 4) & 0xFFFF0);
                        break;
                    case 4:
                        num3 = BM.L2UI((StrToWord(line, 9) << 16) & 0xFFFF0000u);
                        break;
                    case 0:
                        {
                            uint num5 = num4 + num2 + num3;
                            for (int j = 0; j < b; j++)
                            {
                                if ((ulong)(num5 + j) < 65536uL)
                                {
                                    num = CSimMemApp.memApi_SetVal(12289, BM.L2UI(num5 + j), StrToByte(line, 9 + j * 2));
                                    if (num != 0)
                                    {
                                        return 2;
                                    }
                                }
                                else
                                {
                                    num = CSimMemApp.memApi_SetVal(12321, BM.L2UI(num5 - 65536 + j), StrToByte(line, 9 + j * 2));
                                    if (num != 0)
                                    {
                                        return 2;
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            if (num == 0 && !flag2)
            {
                num = 12;
            }
        }
        else
        {
            num = 7;
        }
        return num;
    }

    public int InputFileClassify(string inputFile, ref ulong maxAddr, ref ulong minAddr)
    {
        int result = 0;
        CStdioFileR cStdioFileR = new CStdioFileR();
        ulong num = 0uL;
        ulong num2 = 0uL;
        byte b = 0;
        if (cStdioFileR.OpenForRead(inputFile))
        {
            string line = null;
            bool flag = true;
            ulong num3 = 0uL;
            while (cStdioFileR.ReadString(ref line) && flag)
            {
                if (string.IsNullOrEmpty(line) || line[0] != ':')
                {
                    continue;
                }
                for (int i = 1; i < line.Length; i++)
                {
                    if (!isxdigit(line[i]))
                    {
                        flag = false;
                        return 9;
                    }
                }
                b = StrToByte(line, 1);
                ushort num4 = StrToWord(line, 3);
                switch (StrToByte(line, 7))
                {
                    case 1:
                        flag = false;
                        break;
                    case 2:
                        num = ((ulong)StrToWord(line, 9) << 4) & 0xFFFF0;
                        break;
                    case 4:
                        num2 = ((ulong)StrToWord(line, 9) << 16) & 0xFFFF0000u;
                        break;
                    case 0:
                        num3 = num + num2 + num4;
                        if (num3 + b - 1 > maxAddr)
                        {
                            maxAddr = num3 + b - 1;
                        }
                        if (num3 < minAddr)
                        {
                            minAddr = num3;
                        }
                        break;
                }
            }
        }
        else
        {
            result = 7;
        }
        return result;
    }

    public byte StrToByte(string line, int startPos)
    {
        byte b = 0;
        if (!string.IsNullOrEmpty(line) && startPos + 2 <= line.Length)
        {
            for (int i = startPos; i < startPos + 2; i++)
            {
                b *= 16;
                char c = toupper(line[i]);
                if (isdigit(c))
                {
                    b += BM.I2B(c - 48);
                }
                else if ('A' <= c && c <= 'F')
                {
                    b += BM.I2B(c - 65 + 10);
                }
            }
        }
        return b;
    }

    private ushort StrToWord(string line, int startPos)
    {
        ushort num = 0;
        if (!string.IsNullOrEmpty(line) && startPos + 4 <= line.Length)
        {
            for (int i = startPos; i < startPos + 4; i++)
            {
                num *= 16;
                char c = toupper(line[i]);
                if (isdigit(c))
                {
                    num += BM.I2B(c - 48);
                }
                else if ('A' <= c && c <= 'F')
                {
                    num += BM.I2B(c - 65 + 10);
                }
            }
        }
        return num;
    }

    public bool CheckIntelHexCheckSum(string szHexLine)
    {
        byte b = 0;
        for (int i = 0; i < (szHexLine.Length - 1) / 2; i++)
        {
            b += StrToByte(szHexLine, 1 + i * 2);
        }
        if (b == 0)
        {
            return true;
        }
        return false;
    }

    public int ReadDMemory(uint adrs, byte[] buf, int offset)
    {
        int num = 0;
        byte b = 0;
        if (IsRomWinRange(adrs))
        {
            num = CSimMemApp.memApi_GetVal(12289, adrs, ref b);
            buf[offset] = b;
            if (num != 0)
            {
                return num;
            }
            if (!m_dtword_id || (m_dtword_id && ((adrs & 1) == 0 || !m_dtsize_id)))
            {
                m_ext_rwinsel = true;
                m_rwin_wait_counter++;
            }
        }
        else
        {
            num = CSimMemApp.memApi_GetVal(12305, adrs, ref b);
            buf[offset] = b;
            if (num != 0)
            {
                return num;
            }
        }
        ushort num2 = 0;
        if (num != -2)
        {
            CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
            if (m_SimRunFlag == 0 && (num2 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
            {
                num = 1;
                m_RamNA_brk = 1u;
            }
        }
        return num;
    }

    public int ReadDMemory(uint adrs, ref byte buf)
    {
        byte[] array = new byte[1];
        int result = ReadDMemory(adrs, array, 0);
        buf = array[0];
        return result;
    }

    public int ReadWordDMemory(uint adrs, ref ushort val)
    {
        int num = 0;
        byte lo = 0;
        byte hi = 0;
        if (IsRomWinRange(adrs))
        {
            num = CSimMemApp.memApi_GetVal(12289, adrs, ref lo);
            if (num != 0)
            {
                return num;
            }
            m_ext_rwinsel = true;
            m_rwin_wait_counter++;
        }
        else
        {
            num = CSimMemApp.memApi_GetVal(12305, adrs, ref lo);
            if (num != 0)
            {
                return num;
            }
        }
        ushort num2 = 0;
        if (num != -2)
        {
            CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
            if (m_SimRunFlag == 0 && (num2 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
            {
                num = 1;
                m_RamNA_brk = 1u;
            }
        }
        adrs++;
        if (IsRomWinRange(adrs))
        {
            num = CSimMemApp.memApi_GetVal(12289, adrs, ref hi);
            if (num != 0)
            {
                return num;
            }
            if (m_cpu_mode == 0)
            {
                m_ext_rwinsel = true;
                m_rwin_wait_counter++;
            }
        }
        else
        {
            num = CSimMemApp.memApi_GetVal(12305, adrs, ref hi);
            if (num != 0)
            {
                return num;
            }
        }
        if (num != -2)
        {
            CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
            if (m_SimRunFlag == 0 && (num2 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
            {
                num = 1;
                m_RamNA_brk = 1u;
            }
        }
        val = BM.MAKEWORD(lo, hi);
        return num;
    }

    public int WriteDMemory(uint adrs, byte val)
    {
        int num = 0;
        if (!IsRomWinRange(adrs))
        {
            ushort num2 = 0;
            if (num != -2)
            {
                CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
                if (m_SimRunFlag == 0 && (num2 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
                {
                    m_RamNA_brk = 1u;
                    return 1;
                }
                if (adrs == 61440)
                {
                    m_DSR = val;
                }
            }
            if (num == 0)
            {
                return CSimMemApp.memApi_SetVal(12305, adrs, val);
            }
        }
        else if (!m_dtword_id || (m_dtword_id && ((adrs & 1) == 0 || !m_dtsize_id)))
        {
            m_ext_rwinsel = true;
            m_rwin_wait_counter++;
        }
        return num;
    }

    public int WriteWordDMemory(uint adrs, ushort val)
    {
        int num = 0;
        if (!IsRomWinRange(adrs))
        {
            ushort num2 = 0;
            if (num != -2)
            {
                CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
                if (m_SimRunFlag == 0 && (num2 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
                {
                    m_RamNA_brk = 1u;
                    return 1;
                }
            }
            if (adrs == 61440)
            {
                m_DSR = BM.I2B(val & 0xFF);
            }
            if (num == 0)
            {
                num = CSimMemApp.memApi_SetVal(12305, adrs, BM.I2B(val & 0xFF));
                if (num != 0)
                {
                    return num;
                }
            }
        }
        else
        {
            m_ext_rwinsel = true;
            m_rwin_wait_counter++;
        }
        adrs++;
        if (!IsRomWinRange(adrs))
        {
            ushort num3 = 0;
            if (num != -2)
            {
                CSimDbgApp.dbgApi_GetBreakCondition(ref num3);
                if (m_SimRunFlag == 0 && (num3 & 0x20u) != 0 && CheckDataMapping(adrs) == 0)
                {
                    m_RamNA_brk = 1u;
                    return 1;
                }
            }
            if (adrs == 61440)
            {
                m_DSR = BM.I2B((val >> 8) & 0xFF);
            }
            if (num == 0)
            {
                return CSimMemApp.memApi_SetVal(12305, adrs, BM.I2B((val >> 8) & 0xFF));
            }
        }
        else if (m_cpu_mode == 0)
        {
            m_ext_rwinsel = true;
            m_rwin_wait_counter++;
        }
        return num;
    }

    public int m_Execute()
    {
        int num = 0;
        uint val = 0u;
        uint val2 = 0u;
        uint num2 = 0u;
        ushort num3 = 0;
        if (!m_IdIsEmpty)
        {
            m_ea_plus_ex = false;
            if (!m_ext_dtwait && !EX_wait_req)
            {
                if (m_dstart_id)
                {
                    m_dstart_ex = true;
                }
                if (m_dend_id)
                {
                    m_dend_ex = true;
                }
                m_EXproc();
                if (m_dend_ex && !m_PcChanged)
                {
                    m_Reg.SetReg(16, m_next_pc);
                }
                m_pcbak = m_pc20;
                if (m_Logflag == 2)
                {
                    m_outputLog_1cyc(BM.UI2I(m_pcbak), m_saveIR);
                }
                if (m_edsr_id)
                {
                    m_SetDsrPrefix(1);
                    m_dsr_prifix_inst_ex = true;
                }
                if (m_int_cycle_id)
                {
                    m_int_cycle_ex = true;
                }
            }
            else
            {
                EX_wait_req = false;
            }
        }
        if (m_rwin_wait_counter == 0)
        {
            m_ext_rwinsel = false;
        }
        EX_wait_req = m_ext_pcwait || m_ext_rwinsel;
        if (!m_IfIsEmpty)
        {
            if (!m_ext_dtwait && !m_ext_pcwait && !m_ext_rwinsel && !ID_wait_req)
            {
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                m_IDproc();
                m_Select_excom();
                bool num4 = m_sel_adbus_ea_id || m_sel_a16r_abus_id || m_sel_a16r_ff_id || m_sel_eabus_sp_id;
                if (!m_ea_mode_id && num4)
                {
                    flag3 = true;
                }
                if (m_ea_plus_ex && flag3 && !m_lea_dirct_id)
                {
                    flag = true;
                    m_id_state--;
                }
                EX_wait_req = m_ext_dtwait || m_ext_pcwait || m_ext_rwinsel || flag;
                IF_wait_req = flag || m_pc_wait_id;
                if (!m_PcChanged)
                {
                    m_pc20 = m_next_pc;
                }
            }
            else
            {
                ID_wait_req = false;
            }
        }
        else
        {
            m_CheckInterrupt();
            m_dend_id = true;
        }
        if (m_bcc_id)
        {
            m_Bcctrue_func();
        }
        if (m_PcChanged && m_dend_id)
        {
            m_ReadReg(16, ref val);
            m_ReadReg(21, ref val2);
            m_next_next_pc = ((val2 << 16) & 0xF0000) + (val & 0xFFFF);
            _ = m_next_next_pc;
            _ = m_pcbak;
            m_next_pc = 0u;
            m_id_state = 0;
            m_PcChanged = false;
        }
        if (!m_ext_dtwait && !m_ext_pcwait && !m_ext_rwinsel && !IF_wait_req)
        {
            if (m_IdIsEmpty && m_IfIsEmpty)
            {
                m_ReadReg(16, ref val);
                m_ReadReg(21, ref val2);
                m_next_next_pc = ((val2 << 16) & 0xF0000) + (val & 0xFFFF);
            }
            m_IFproc();
            m_next_pc = m_next_next_pc;
            m_next_next_pc = (m_next_next_pc & 0xF0000) + ((m_next_next_pc + 2) & 0xFFFF);
        }
        else
        {
            IF_wait_req = false;
        }
        if (m_IdIsEmpty)
        {
            m_IR = m_pcrdata;
        }
        if (!(m_ext_dtwait | m_ext_pcwait | m_ext_rwinsel))
        {
            m_IRproc_afterCycle(m_pcrdata);
        }
        if (m_dend_id || m_state_clr_id)
        {
            m_id_state = 0;
        }
        if (m_dend_ex)
        {
            m_ex_state = 0;
        }
        m_ChangeCPUMode();
        byte b = 0;
        ulong num5 = 1uL;
        if (m_GetDsrPrefix() == 1)
        {
            num5++;
        }
        for (int i = 0; i < m_InterruptNum; i++)
        {
            if (!m_InterruptSetting[i].autoInterrupt)
            {
                continue;
            }
            if (m_InterruptSetting[i].onceOption)
            {
                if (m_InterruptSetting[i].onceMode == 0)
                {
                    if (m_InterruptSetting[i].onceAdrs == m_pcbak)
                    {
                        CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[i].irq_adrs, ref b);
                        b |= m_InterruptInfo[i].irq_mask;
                        CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[i].irq_adrs, b);
                        m_InterruptAuto[i].onceFlag = true;
                    }
                }
                else if (m_InterruptAuto[i].onceCount <= num5)
                {
                    CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[i].irq_adrs, ref b);
                    b |= m_InterruptInfo[i].irq_mask;
                    CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[i].irq_adrs, b);
                    m_InterruptAuto[i].onceFlag = true;
                }
                else
                {
                    m_InterruptAuto[i].onceCount -= num5;
                }
            }
            if (m_InterruptSetting[i].repeatOption)
            {
                if (m_InterruptAuto[i].repeatCount <= num5)
                {
                    CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[i].irq_adrs, ref b);
                    b |= m_InterruptInfo[i].irq_mask;
                    CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[i].irq_adrs, b);
                    SetAutoIntCount(i);
                }
                else
                {
                    m_InterruptAuto[i].repeatCount -= num5;
                }
            }
        }
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        CSimDbgApp.dbgApi_SetBreakStatus((ushort)0);
        byte b2 = 0;
        uint num6 = 0u;
        CSimDbgApp.dbgApi_GetIEVal(m_pcbak, ref b2);
        if (m_pcbak >= m_CodeStartAdr && m_pcbak <= m_CodeEndAdr && b2 == 0 && m_dstart_ex && !m_int_cycle_ex)
        {
            CSimDbgApp.dbgApi_SetIEVal(m_pcbak, 1);
            CSimDbgApp.dbgApi_SetIEVal(m_pcbak + 1, 1);
            if (m_pcbak >= m_CvCntAddressStart && m_pcbak <= m_CvCntAddressEnd && m_pcbak + 1 <= m_CvCntAddressEnd)
            {
                CSimDbgApp.dbgApi_GetIECount(ref num6);
                num6 += 2;
                CSimDbgApp.dbgApi_SetIECount(num6);
            }
        }
        if (m_dstart_ex)
        {
            byte[] val3 = ICESWI_CODE();
            if (m_RestartFlag)
            {
                m_RestartFlag = false;
                m_WriteCodeMemory(m_pcbak, 2u, val3);
            }
        }
        if (m_dend_ex)
        {
            m_gcount++;
        }
        if (m_CycleCountEnable >= 3)
        {
            TRMEM tRMEM = default(TRMEM);
            ushort num7 = 0;
            uint num8 = 0u;
            byte b3 = 0;
            byte b4 = 0;
            byte b5 = 0;
            byte b6 = 0;
            b5 = BM.I2B((m_dstart_ex && (m_GetDsrPrefix() == 1 || !m_dsr_prifix_inst_ex)) ? 1 : 0);
            b4 = BM.I2B(m_int_cycle_ex ? 1 : 0);
            b6 = BM.I2B((m_TraceLastMemDat.byte_h_l << 6) + (b5 << 1) + b4);
            tRMEM.pc = ((b5 != 0) ? m_pcbak : m_execute_pc);
            m_execute_pc = tRMEM.pc;
            tRMEM.psw = BM.I2W(m_Reg.GetReg(26));
            tRMEM.ramad = m_TraceLastMemAdr;
            tRMEM.probe = 0;
            if (m_cpu_mode == 0)
            {
                tRMEM.ramdt = BM.UI2B(m_TraceLastMemDat.data);
                tRMEM.ramdt16 = 0;
                tRMEM.intcycle = b4;
                tRMEM.atr = b5;
            }
            else
            {
                tRMEM.ramdt = 0;
                tRMEM.ramdt16 = BM.UI2W(m_TraceLastMemDat.data);
                tRMEM.intcycle = b4;
                tRMEM.atr = b6;
            }
            CSimDbgApp.dbgApi_GetTracePointer(ref num8, ref b3);
            CSimDbgApp.dbgApi_WriteTraceMemory(num8, 1, ref num7, ref tRMEM);
            m_TraceBreakPointer++;
        }
        if (m_dend_ex)
        {
            ushort cond = 0;
            if (m_SimRunFlag == 16)
            {
                m_BreakPC = m_pcbak;
                m_SimRunFlag = 10;
                CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                num2 |= 0x80u;
                CSimDbgApp.dbgApi_SetBreakStatus(num2);
            }
            if (m_GetDsrPrefix() != 2)
            {
                m_GetBreakCondition(ref cond);
                if ((cond & 2u) != 0)
                {
                    uint num9 = 0u;
                    CSimDbgApp.dbgApi_GetTraceCountBP(ref num9);
                    if (num9 <= m_TraceBreakPointer)
                    {
                        m_SimRunFlag = 11;
                        m_NextPC = m_pc20;
                        m_BreakPC = m_pcbak;
                        CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                        num2 |= 0x100u;
                        CSimDbgApp.dbgApi_SetBreakStatus(num2);
                        CSimDbgApp.dbgApi_SetTraceCountBP(num9);
                        m_TraceBreakPointer = 0u;
                    }
                }
            }
        }
        if (m_dend_ex)
        {
            if (m_RamNA_brk != 0)
            {
                m_SimRunFlag = 14;
                m_NextPC = m_pc20;
                m_BreakPC = m_pcbak;
                CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                num2 |= 0x800u;
                CSimDbgApp.dbgApi_SetBreakStatus(num2);
                m_RamNA_brk = 0u;
            }
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            byte[] array = new byte[1];
            byte[] array2 = new byte[2];
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
            if (m_GetDsrPrefix() == 0 && (bRKPARAM.brkcond & 2u) != 0)
            {
                switch (bRKPARAM.dm_pair & 7)
                {
                    case 0:
                        if (!m_bDMand1 && !m_bDMand2)
                        {
                            m_bDMand1 = checkDM(0, saveflg: true);
                            m_bDMand2 = checkDM(2, saveflg: true);
                            flag4 = m_bDMand1 && m_bDMand2;
                            break;
                        }
                        if (!m_bDMand1)
                        {
                            m_bDMand1 = checkDM(0);
                            flag6 = m_bDMand1;
                        }
                        else
                        {
                            m_bDMand2 = checkDM(2);
                            flag7 = m_bDMand2;
                        }
                        if (!(flag6 || flag7))
                        {
                            break;
                        }
                        m_ReadDataMemory(m_DMadrs, 1u, array);
                        flag4 = ((array[0] == m_DMbval) ? true : false);
                        if (!flag4)
                        {
                            if (flag6)
                            {
                                m_bDMand2 = false;
                                m_bDMand1 = checkDM(0, saveflg: true);
                            }
                            else
                            {
                                m_bDMand1 = false;
                                m_bDMand2 = checkDM(2, saveflg: true);
                            }
                        }
                        break;
                    case 1:
                        flag4 = checkDM(1) || checkDM(3);
                        break;
                    case 2:
                        if (!m_bDMand1 && !m_bDMand2)
                        {
                            m_bDMand1 = checkDM(1, saveflg: true);
                            m_bDMand2 = checkDM(2, saveflg: true);
                            flag4 = m_bDMand1 && m_bDMand2;
                            break;
                        }
                        if (!m_bDMand1)
                        {
                            m_bDMand1 = checkDM(1);
                            if (m_bDMand1)
                            {
                                m_ReadDataMemory(m_DMadrs, 1u, array);
                                flag4 = ((array[0] == m_DMbval) ? true : false);
                                if (!flag4)
                                {
                                    m_bDMand2 = false;
                                    m_bDMand1 = checkDM(1, saveflg: true);
                                }
                            }
                            break;
                        }
                        m_bDMand2 = checkDM(2);
                        if (m_bDMand2)
                        {
                            m_ReadDataMemory(m_DMadrs, 2u, array2);
                            flag4 = ((BM.MAKEWORD(array2[0], array2[1]) == m_DMwval) ? true : false);
                            if (!flag4)
                            {
                                m_bDMand1 = false;
                                m_bDMand2 = checkDM(2, saveflg: true);
                            }
                        }
                        break;
                    case 3:
                        if (!m_bDMand1 && !m_bDMand2)
                        {
                            m_bDMand1 = checkDM(1, saveflg: true);
                            m_bDMand2 = checkDM(3, saveflg: true);
                            flag4 = m_bDMand1 && m_bDMand2;
                            break;
                        }
                        if (!m_bDMand1)
                        {
                            m_bDMand1 = checkDM(1);
                            flag6 = m_bDMand1;
                        }
                        else
                        {
                            m_bDMand2 = checkDM(3);
                            flag7 = m_bDMand2;
                        }
                        if (!(flag6 || flag7))
                        {
                            break;
                        }
                        m_ReadDataMemory(m_DMadrs, 2u, array2);
                        flag4 = ((BM.MAKEWORD(array2[0], array2[1]) == m_DMwval) ? true : false);
                        if (!flag4)
                        {
                            if (flag6)
                            {
                                m_bDMand2 = false;
                                m_bDMand1 = checkDM(1, saveflg: true);
                            }
                            else
                            {
                                m_bDMand1 = false;
                                m_bDMand2 = checkDM(3, saveflg: true);
                            }
                        }
                        break;
                    case 4:
                        flag4 = checkDM(1) || checkDM(2);
                        break;
                    case 5:
                        flag4 = checkDM(0) || checkDM(2);
                        flag5 = flag4;
                        break;
                    case 6:
                        flag4 = ((bRKPARAM.dm_param[0].ramadrsmask == 0) ? checkDM(2) : checkDM(0));
                        flag5 = flag4;
                        break;
                    case 7:
                        flag4 = ((bRKPARAM.dm_param[1].ramadrsmask == 0) ? checkDM(3) : checkDM(1));
                        break;
                }
                if (flag5)
                {
                    if (bRKPARAM.dm_pcnt != 1)
                    {
                        flag4 = false;
                        if (bRKPARAM.dm_pcnt == 0)
                        {
                            bRKPARAM.dm_pcnt = ushort.MaxValue;
                            CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
                        }
                        else
                        {
                            bRKPARAM.dm_pcnt--;
                            CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
                        }
                    }
                    else
                    {
                        bRKPARAM.dm_pcnt = 0;
                        CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
                    }
                }
                if (flag4)
                {
                    m_SimRunFlag = 9;
                    m_NextPC = m_pc20;
                    m_BreakPC = m_pcbak;
                    CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                    num2 |= 8u;
                    CSimDbgApp.dbgApi_SetBreakStatus(num2);
                }
            }
            if (m_SimRunFlag != 15)
            {
                CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
                if (((uint)bRKPARAM.brkcond & (true ? 1u : 0u)) != 0 && m_pc20 == bRKPARAM.adrbrk_adrs)
                {
                    bRKPARAM.adrbrk_pcnt--;
                    CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
                    if (bRKPARAM.adrbrk_pcnt == 0)
                    {
                        m_SimRunFlag = 8;
                        m_NextPC = m_pc20;
                        m_BreakPC = m_pc20;
                        CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                        num2 |= 2u;
                        CSimDbgApp.dbgApi_SetBreakStatus(num2);
                    }
                }
            }
            if (m_GetDsrPrefix() == 0)
            {
                if (m_StepFlag == 1)
                {
                    m_StepFlag = 0;
                    m_SimRunFlag = 6;
                    m_NextPC = m_pc20;
                    m_BreakPC = m_pcbak;
                    CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                    num2 |= 0x40u;
                    CSimDbgApp.dbgApi_SetBreakStatus(num2);
                }
                else if (m_StepFlag == 2)
                {
                    if (m_StepInst == 1)
                    {
                        m_StepFlag = 3;
                    }
                    else if (m_int_cycle_ex)
                    {
                        if (m_int_cycle_id)
                        {
                            m_StepFlag = 2;
                        }
                        else
                        {
                            m_StepFlag = 3;
                            m_StepOverPC = (uint)m_retNextPC;
                        }
                    }
                    else if (m_int_cycle_id)
                    {
                        m_StepFlag = 2;
                    }
                    else
                    {
                        m_StepFlag = 0;
                        m_SimRunFlag = 6;
                        m_NextPC = m_pc20;
                        m_BreakPC = m_pcbak;
                        CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                        num2 |= 0x40u;
                        CSimDbgApp.dbgApi_SetBreakStatus(num2);
                    }
                }
                else if (m_StepFlag == 3)
                {
                    uint num10 = (uint)m_Reg.GetReg(16);
                    if (m_MemoryModel != 0)
                    {
                        num10 += (uint)(m_Reg.GetReg(21) << 16);
                    }
                    if (m_int_cycle_id)
                    {
                        m_StepFlag = 2;
                    }
                    else if (num10 == m_StepOverPC)
                    {
                        m_StepFlag = 0;
                        m_SimRunFlag = 6;
                        m_NextPC = num10;
                        m_BreakPC = m_pcbak;
                        CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                        num2 |= 0x40u;
                        CSimDbgApp.dbgApi_SetBreakStatus(num2);
                    }
                }
            }
            CSimDbgApp.dbgApi_GetBreakCondition(ref num3);
            if (m_GetDsrPrefix() == 0 && ((uint)num3 & (true ? 1u : 0u)) != 0 && (m_SimRunFlag == 1 || m_SimRunFlag == 2))
            {
                m_SimRunFlag = 12;
                m_NextPC = m_pc20;
                m_BreakPC = m_pcbak;
                CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
                num2 |= 1u;
                CSimDbgApp.dbgApi_SetBreakStatus(num2);
            }
            RdRamCnt = 0;
            TrcRamCnt = 0;
        }
        if (m_iceswi_id)
        {
            m_SimRunFlag = 5;
            m_NextPC = m_pc20;
            m_BreakPC = m_pc20;
            CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
            num2 |= 4u;
            CSimDbgApp.dbgApi_SetBreakStatus(num2);
        }
        if (m_RomNA_brk != 0)
        {
            m_SimRunFlag = 13;
            CSimDbgApp.dbgApi_GetBreakStatus(ref num2);
            num2 |= 0x400u;
            CSimDbgApp.dbgApi_SetBreakStatus(num2);
            m_RomNA_brk = 0u;
        }
        if (m_CycleCountEnable >= 3)
        {
            ulong num11 = 0uL;
            byte b7 = 0;
            if (num != -2)
            {
                CSimDbgApp.dbgApi_GetCycleCounter(ref num11, ref b7);
                num11++;
                if (m_inst_DI)
                {
                    if (m_core_rev_type == 0)
                    {
                        m_DI_cycle = 1u;
                    }
                    else
                    {
                        m_DI_cycle = 3u;
                    }
                    num11 += m_DI_cycle - 3;
                }
                CSimDbgApp.dbgApi_SetCycleCounter(num11);
                m_beforeCycleCounter = num11;
            }
        }
        if (m_SimRunFlag != 0 && m_SimRunFlag != 3 && m_SimRunFlag != 15 && m_SimRunFlag != 16)
        {
            if (m_rwin_wait_counter != 0)
            {
                ulong num12 = 0uL;
                byte b8 = 0;
                CSimDbgApp.dbgApi_GetCycleCounter(ref num12, ref b8);
                num12 += (ulong)m_rwin_wait_counter;
                CSimDbgApp.dbgApi_SetCycleCounter(num12);
                m_ea_plus_ex = false;
            }
            m_IfIsEmpty = true;
            m_IdIsEmpty = true;
            m_id_state = 0;
            m_ex_state = 0;
            m_rwin_wait_counter = 0;
            m_ext_rwinsel = false;
            m_ClearIdSetFlg();
        }
        if (m_rwin_wait_counter > 0)
        {
            m_rwin_wait_counter--;
        }
        m_ext_dtwait = false;
        m_ext_pcwait = false;
        m_ClearExSetFlg();
        return 0;
    }

    public bool m_CheckInterrupt()
    {
        byte b = 0;
        byte b2 = 0;
        bool result = false;
        _ = new INTNUM[32];
        if (0 == 0)
        {
            byte b3 = byte.MaxValue;
            for (byte b4 = 0; b4 < m_InterruptNum; b4++)
            {
                CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[b4].irq_adrs, ref b);
                if ((b & m_InterruptInfo[b4].irq_mask) != 0)
                {
                    b3 = b4;
                }
                if (b3 < m_InterruptNum)
                {
                    CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[b3].ie_adrs, ref b2);
                    if (m_InterruptInfo[b3].ie_adrs == 0 || (b2 & m_InterruptInfo[b3].ie_mask) != 0)
                    {
                        byte b5 = 0;
                        byte pSW = m_Reg.GetPSW(54);
                        ushort num = (ushort)m_InterruptInfo[b3].vector_adrs;
                        if (2 <= num && num <= 5)
                        {
                            m_reset_req = true;
                            b5 = 1;
                        }
                        else if (6 <= num && num <= 7)
                        {
                            if (pSW <= 3)
                            {
                                m_nmice_req = true;
                                b5 = 1;
                            }
                        }
                        else if (8 <= num && num <= 127)
                        {
                            if (m_InterruptInfo[b3].ie_adrs == 0)
                            {
                                if (pSW <= 2)
                                {
                                    m_nmi_req = true;
                                    b5 = 1;
                                }
                            }
                            else if (pSW <= 1)
                            {
                                m_mi_req = true;
                                b5 = 1;
                            }
                        }
                        else if (pSW <= 1)
                        {
                            m_swi_req = true;
                            b5 = 1;
                        }
                        if (b5 != 0)
                        {
                            m_vector = num;
                            result = true;
                            m_current_irqno = b3;
                        }
                        else
                        {
                            m_reset_req = false;
                            m_nmice_req = false;
                            m_nmi_req = false;
                            m_mi_req = false;
                            m_swi_req = false;
                        }
                        break;
                    }
                    m_reset_req = false;
                    m_nmice_req = false;
                    m_nmi_req = false;
                    m_mi_req = false;
                    m_swi_req = false;
                    m_STPACPflag = 0;
                }
                else
                {
                    m_reset_req = false;
                    m_nmice_req = false;
                    m_nmi_req = false;
                    m_mi_req = false;
                    m_swi_req = false;
                }
            }
        }
        return result;
    }

    public int m_CheckInterruptStopHalt()
    {
        byte b = 0;
        byte b2 = 0;
        if (0 == 0)
        {
            byte b3 = byte.MaxValue;
            for (byte b4 = 0; b4 < m_InterruptNum; b4++)
            {
                CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[b4].irq_adrs, ref b);
                if ((b & m_InterruptInfo[b4].irq_mask) != 0)
                {
                    b3 = b4;
                }
                if (b3 < m_InterruptNum)
                {
                    CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[b3].ie_adrs, ref b2);
                    if (m_InterruptInfo[b3].ie_adrs == 0 || (b2 & m_InterruptInfo[b3].ie_mask) != 0)
                    {
                        m_SimRunFlag = 0;
                        m_ClearSBYCON();
                        m_STPACPflag = 0;
                        break;
                    }
                    m_STPACPflag = 0;
                }
            }
        }
        return 0;
    }

    public void InitSetting()
    {
        uint num = 0u;
        uint num2 = 0u;
        CSimMemApp.memApi_GetRange(12289, ref num, ref num2);
        if (num > num2)
        {
            CSimMemApp.memApi_SetRange(12289, 0, 65535);
            CSimMemApp.memApi_SetRange(12321, 0, 983039);
            CSimDbgApp.dbgApi_SetIERange(0, 983039);
            m_CodeStartAdr = 0u;
            m_CodeEndAdr = 1048575u;
            if (m_Logflag == 1)
            {
                new COutMod().W(" InitSetting:").endl();
                new COutMod().W(" Code Range: ").hex(m_CodeStartAdr).W(" - ")
                    .hex(m_CodeEndAdr)
                    .endl();
                CSimMemApp.memApi_GetRange(12289, ref num, ref num2);
                new COutMod().W(" CMem Start: ").hex(num).W("  End: ")
                    .hex(num2)
                    .endl();
                CSimMemApp.memApi_GetRange(12321, ref num, ref num2);
                new COutMod().W(" GMem Start: ").hex(num).W("  End: ")
                    .hex(num2)
                    .endl();
            }
        }
        CSimMemApp.memApi_GetRange(12305, ref num, ref num2);
        if (num > num2)
        {
            CSimMemApp.memApi_SetRange(12305, 0, 65535);
            CSimMemApp.memApi_SetRange(12321, 0, 16711679);
            m_DataStartAdr = 0u;
            m_DataEndAdr = 16777215u;
            if (m_Logflag == 1)
            {
                new COutMod().W(" InitSetting:").endl();
                new COutMod().W(" Data Range: ").hex(m_DataStartAdr).W(" - ")
                    .hex(m_DataEndAdr)
                    .endl();
                CSimMemApp.memApi_GetRange(12305, ref num, ref num2);
                new COutMod().W(" DMem Start: ").hex(num).W("  End: ")
                    .hex(num2)
                    .endl();
                CSimMemApp.memApi_GetRange(12321, ref num, ref num2);
                new COutMod().W(" GMem Start: ").hex(num).W("  End: ")
                    .hex(num2)
                    .endl();
            }
        }
        if (m_RomWinStartAdr > m_RomWinEndAdr)
        {
            m_RomWinStartAdr = 0u;
            m_RomWinEndAdr = 32767u;
            if (m_Logflag == 1)
            {
                new COutMod().W(" InitSetting:").endl();
                new COutMod().W(" RomWindow Range: ").hex(m_RomWinStartAdr).W(" - ")
                    .hex(m_RomWinEndAdr)
                    .endl();
            }
        }
        if (m_DefaultCode == -1)
        {
            m_SetCodeMemoryDefaultCode(ushort.MaxValue);
        }
        m_Initflag = true;
    }

    public bool IsRomWinRange(uint adr)
    {
        if (adr < m_RomWinStartAdr || adr > m_RomWinEndAdr)
        {
            return false;
        }
        if (m_RomWinStartAdr == 0 && m_RomWinEndAdr == 0)
        {
            return false;
        }
        return true;
    }

    public bool IsRomWinRange_GMem(uint adr)
    {
        bool result = false;
        byte b = 0;
        b = CheckDataMapping(adr);
        if (b == 7 || b == 8)
        {
            result = true;
        }
        return result;
    }

    public int m_SetMappingMIO(ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        if (n < 1 || n > 16)
        {
            return -1;
        }
        uint num = 65535u;
        CSimMemApp.memApi_SetCount(12289, (byte)n);
        for (byte b = 0; b < n; b++)
        {
            uint num2 = startadrs[b];
            uint num3 = endadrs[b];
            if (num2 > num3)
            {
                return -1;
            }
            byte b2 = atrb[b];
            if (b2 == 5 || b2 == 6 || b2 > 11)
            {
                return -1;
            }
            CSimMemApp.memApi_SetStartAddress(12289, b, num2);
            CSimMemApp.memApi_SetEndAddress(12289, b, num3);
            CSimMemApp.memApi_SetAttribute(12289, b, b2);
            if (b2 != 0)
            {
                num = endadrs[b];
            }
        }
        if (num > m_CodeEndAdr && m_SetCodeMemorySize(0u, num) == -2)
        {
            return -2;
        }
        return 0;
    }

    public int m_GetMappingMIO(ref ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        byte b = 0;
        CSimMemApp.memApi_GetCount(12289, ref b);
        n = b;
        for (byte b2 = 0; b2 < n; b2++)
        {
            CSimMemApp.memApi_GetStartAddress(12289, b2, startadrs, b2);
            CSimMemApp.memApi_GetEndAddress(12289, b2, endadrs, b2);
            CSimMemApp.memApi_GetAttribute(12289, b2, atrb, b2);
        }
        return 0;
    }

    public int m_SetMappingPIO(ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        if (n < 1 || n > 16)
        {
            return -1;
        }
        uint endadrs2 = 65535u;
        CSimMemApp.memApi_SetCount(12305, (byte)n);
        for (byte b = 0; b < n; b++)
        {
            uint num = startadrs[b];
            uint num2 = endadrs[b];
            if (num > num2)
            {
                return -1;
            }
            byte b2 = atrb[b];
            if ((b2 >= 5 && b2 <= 11) || b2 > 13)
            {
                return -1;
            }
            CSimMemApp.memApi_SetStartAddress(12305, b, num);
            CSimMemApp.memApi_SetEndAddress(12305, b, num2);
            CSimMemApp.memApi_SetAttribute(12305, b, b2);
            if (b2 != 0)
            {
                endadrs2 = endadrs[b];
            }
        }
        if (m_SetDataMemorySize(0u, endadrs2) == -2)
        {
            return -2;
        }
        return 0;
    }

    public int m_GetMappingPIO(ref ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        byte b = 0;
        CSimMemApp.memApi_GetCount(12305, ref b);
        n = b;
        for (byte b2 = 0; b2 < n; b2++)
        {
            CSimMemApp.memApi_GetStartAddress(12305, b2, startadrs, b2);
            CSimMemApp.memApi_GetEndAddress(12305, b2, endadrs, b2);
            CSimMemApp.memApi_GetAttribute(12305, b2, atrb, b2);
        }
        return 0;
    }

    public int m_SetMappingGIO(ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        if (n < 1 || n > 16)
        {
            return -1;
        }
        uint num = 1048575u;
        CSimMemApp.memApi_SetCount(12321, (byte)n);
        for (byte b = 0; b < n; b++)
        {
            uint num2 = startadrs[b];
            uint num3 = endadrs[b];
            if (num2 > num3)
            {
                return -1;
            }
            byte b2 = atrb[b];
            if (b2 > 11)
            {
                return -1;
            }
            CSimMemApp.memApi_SetStartAddress(12321, b, num2);
            CSimMemApp.memApi_SetEndAddress(12321, b, num3);
            CSimMemApp.memApi_SetAttribute(12321, b, b2);
            if (b2 != 0)
            {
                num = endadrs[b];
            }
        }
        if (m_SetCodeMemorySize(0u, (num > 1048575) ? 1048575u : num) == -2)
        {
            return -2;
        }
        if (m_SetDataMemorySize(0u, num) == -2)
        {
            return -2;
        }
        return 0;
    }

    public int m_GetMappingGIO(ref ushort n, uint[] startadrs, uint[] endadrs, byte[] atrb)
    {
        byte b = 0;
        CSimMemApp.memApi_GetCount(12321, ref b);
        n = b;
        for (byte b2 = 0; b2 < n; b2++)
        {
            CSimMemApp.memApi_GetStartAddress(12321, b2, startadrs, b2);
            CSimMemApp.memApi_GetEndAddress(12321, b2, endadrs, b2);
            CSimMemApp.memApi_GetAttribute(12321, b2, atrb, b2);
        }
        return 0;
    }

    public int m_FillMemoryU8(ushort rID, uint startadrs, uint endadrs, ushort size, uint val)
    {
        uint num = 0u;
        uint num2 = 0u;
        if (rID == 12323)
        {
            CSimDbgApp.dbgApi_GetIERange(ref num, ref num2);
        }
        else
        {
            CSimMemApp.memApi_GetRange(rID, ref num, ref num2);
        }
        if (num > num2)
        {
            return -1;
        }
        byte[] array = new byte[4]
        {
            BM.UI2B(val & 0xFFu),
            BM.UI2B((val >> 8) & 0xFFu),
            BM.UI2B((val >> 16) & 0xFFu),
            BM.UI2B((val >> 24) & 0xFFu)
        };
        for (uint num3 = startadrs; num3 < endadrs; num3 += size)
        {
            for (uint num4 = 0u; num4 < size; num4++)
            {
                if (rID != 12323)
                {
                    CSimMemApp.memApi_SetVal(rID, num3 + num4, array[num4]);
                }
                else
                {
                    CSimDbgApp.dbgApi_SetIEVal(num3 + num4, array[num4]);
                }
            }
        }
        return 0;
    }

    public int m_SetBreakPoint(ushort n, uint[] adrs, ref ushort num, ushort[] code)
    {
        byte[] val = ICESWI_CODE();
        byte[] array = new byte[1];
        byte[] array2 = new byte[1];
        int num2 = 0;
        if (n == 0)
        {
            num = 0;
            return -1;
        }
        for (ushort num3 = 0; num3 < n; num3++)
        {
            uint num4 = adrs[num3];
            num2 = m_ReadCodeMemory(num4, 1u, array);
            if (num2 != 0)
            {
                return num2;
            }
            num2 = m_ReadCodeMemory(num4 + 1, 1u, array2);
            if (num2 != 0)
            {
                return num2;
            }
            code[num3] = BM.I2W((array2[0] << 8) + array[0]);
            if (m_WriteCodeMemory(num4, 2u, val) == -2)
            {
                return -2;
            }
        }
        num = n;
        return 0;
    }

    public int m_ClearBreakPoint(ushort n, uint[] adrs, ushort[] code)
    {
        byte[] array = new byte[2];
        int num = 0;
        for (ushort num2 = 0; num2 < n; num2++)
        {
            uint adrs2 = adrs[num2];
            array[0] = BM.I2B(code[num2] & 0xFF);
            array[1] = BM.I2B((code[num2] >> 8) & 0xFF);
            num = m_WriteCodeMemory(adrs2, 2u, array);
            if (num != 0)
            {
                return num;
            }
        }
        return 0;
    }

    public int m_GetBreakStatus(ref uint status)
    {
        CSimDbgApp.dbgApi_GetBreakStatus(ref status);
        return 0;
    }

    public int m_SetBreakCondition(ushort cond)
    {
        ushort num = BM.UI2W(cond & 0x3Fu);
        CSimDbgApp.dbgApi_SetBreakCondition(num);
        return 0;
    }

    public int m_GetBreakCondition(ref ushort cond)
    {
        if (!m_Initflag)
        {
            InitSetting();
        }
        CSimDbgApp.dbgApi_GetBreakCondition(ref cond);
        return 0;
    }

    public int m_SimStart_Break(ref BRKPARAM param)
    {
        int num = 0;
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        int num2 = m_Reg.GetReg(16);
        if (m_MemoryModel != 0)
        {
            num2 += m_Reg.GetReg(21) << 16;
        }
        if (num != -2)
        {
            CSimDbgApp.dbgApi_SetBreakParam(ref param);
        }
        m_SimRunFlag = 0;
        if (num != -2)
        {
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
            if (((uint)bRKPARAM.brkcond & (true ? 1u : 0u)) != 0 && num2 == bRKPARAM.adrbrk_adrs)
            {
                m_SimRunFlag = 15;
            }
        }
        m_bDMand1 = false;
        m_bDMand2 = false;
        m_DMadrs = 0u;
        m_DMbval = 0;
        m_DMwval = 0;
        m_RestartFlag = false;
        m_rwin_wait_counter = 0;
        m_ext_rwinsel = false;
        return num;
    }

    public int m_SimStart_Restart(ushort code, BRKPARAM param)
    {
        int num = 0;
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        byte[] array = new byte[2];
        uint num2 = BM.I2UI(m_Reg.GetReg(16));
        if (m_MemoryModel != 0)
        {
            num2 += BM.I2UI(m_Reg.GetReg(21)) << 16;
        }
        array[0] = BM.I2B(code & 0xFF);
        array[1] = BM.I2B((code >> 8) & 0xFF);
        num = m_WriteCodeMemory(num2, 2u, array);
        if (num == -2)
        {
            return -2;
        }
        if (num != -2)
        {
            CSimDbgApp.dbgApi_SetBreakParam(ref param);
        }
        m_SimRunFlag = 0;
        if (num != -2)
        {
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
            if (((uint)bRKPARAM.brkcond & (true ? 1u : 0u)) != 0 && num2 == bRKPARAM.adrbrk_adrs)
            {
                m_SimRunFlag = 15;
            }
        }
        m_bDMand1 = false;
        m_bDMand2 = false;
        m_DMadrs = 0u;
        m_DMbval = 0;
        m_DMwval = 0;
        m_RestartFlag = true;
        m_rwin_wait_counter = 0;
        m_ext_rwinsel = false;
        return num;
    }

    public int m_StepIn()
    {
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        int num = 0;
        m_StepFlag = 1;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
            bRKPARAM.adrbrk_adrs = 0u;
            bRKPARAM.adrbrk_pcnt = 1;
            for (int i = 0; i < 4; i++)
            {
                bRKPARAM.dm_param[i].ramadrs = 0u;
                bRKPARAM.dm_param[i].ramadrsmask = uint.MaxValue;
                bRKPARAM.dm_param[i].ramdata = 0;
                bRKPARAM.dm_param[i].ramdatamask = byte.MaxValue;
                bRKPARAM.dm_param[i].condition = 1;
                bRKPARAM.dm_param[i].access = 0;
            }
            bRKPARAM.dm_pcnt = 1;
            bRKPARAM.dm_pair = 6;
            bRKPARAM.brkcond = 0;
            CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
        }
        m_SimRunFlag = 0;
        m_RestartFlag = false;
        m_rwin_wait_counter = 0;
        m_ext_rwinsel = false;
        return num;
    }

    public int m_StepOver()
    {
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        int num = 0;
        m_StepFlag = 2;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
            bRKPARAM.adrbrk_adrs = 0u;
            bRKPARAM.adrbrk_pcnt = 1;
            for (int i = 0; i < 4; i++)
            {
                bRKPARAM.dm_param[i].ramadrs = 0u;
                bRKPARAM.dm_param[i].ramadrsmask = uint.MaxValue;
                bRKPARAM.dm_param[i].ramdata = 0;
                bRKPARAM.dm_param[i].ramdatamask = byte.MaxValue;
                bRKPARAM.dm_param[i].condition = 1;
                bRKPARAM.dm_param[i].access = 0;
            }
            bRKPARAM.dm_pcnt = 1;
            bRKPARAM.dm_pair = 6;
            bRKPARAM.brkcond = 0;
            CSimDbgApp.dbgApi_SetBreakParam(ref bRKPARAM);
        }
        m_SimRunFlag = 0;
        m_RestartFlag = false;
        m_rwin_wait_counter = 0;
        m_ext_rwinsel = false;
        return num;
    }

    public int m_CheckBreakGo(ref byte status, ref uint nextpc, ref uint breakpc, ref uint brkstatus, ref ushort adr_brk_passcnt, ref ushort rammatch_brk_passcnt)
    {
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        uint num = 0u;
        int num2 = 0;
        if (m_SimRunFlag >= 5 && m_SimRunFlag <= 14)
        {
            status = 1;
            nextpc = m_NextPC;
            breakpc = m_BreakPC;
            if (num2 != -2)
            {
                CSimDbgApp.dbgApi_GetBreakStatus(ref num);
                CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
                brkstatus = num;
                adr_brk_passcnt = bRKPARAM.adrbrk_pcnt;
                rammatch_brk_passcnt = bRKPARAM.dm_pcnt;
            }
        }
        else
        {
            status = 0;
        }
        return num2;
    }

    public int m_CheckBreakStep(ref byte status, ref uint nextpc, ref uint breakpc, ref uint brkstatus)
    {
        uint num = 0u;
        int num2 = 0;
        if (m_SimRunFlag >= 5 && m_SimRunFlag <= 14)
        {
            status = 1;
            nextpc = m_NextPC;
            breakpc = m_BreakPC;
            if (num2 != -2)
            {
                CSimDbgApp.dbgApi_GetBreakStatus(ref num);
                brkstatus = num;
            }
        }
        else
        {
            status = 0;
        }
        return num2;
    }

    public int m_ReadTraceMemory(uint tp, ushort n, ref ushort m, ref TRMEM[] tracedat)
    {
        int num = 0;
        if (num == 0)
        {
            num = CSimDbgApp.dbgApi_ReadTraceMemory(tp, n, ref m, tracedat);
        }
        return num;
    }

    public int m_GetTracePointer(ref uint tp, ref byte buf1)
    {
        int num = 0;
        uint num2 = 0u;
        byte b = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetTracePointer(ref num2, ref b);
            if (b != 0)
            {
                tp = 262143u;
            }
            else
            {
                tp = num2;
            }
            buf1 = b;
        }
        return num;
    }

    public int m_SetTraceCountBP(uint cnt)
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_SetTraceCountBP(cnt);
        }
        return num;
    }

    public int m_GetTraceCountBP(ref uint cnt)
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetTraceCountBP(ref cnt);
        }
        return num;
    }

    public int m_SearchTraceMemory(byte sign, ushort rID, uint compdata, uint mask, uint cnt, ref uint tp)
    {
        int num = 0;
        if (num == 0)
        {
            num = CSimDbgApp.dbgApi_SearchTraceMemory(sign, rID, compdata, mask, cnt, ref tp);
        }
        return num;
    }

    public int m_ClearTracePointer()
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_ClearTracePointer();
        }
        m_TraceBreakPointer = 0u;
        return num;
    }

    public int m_GetCycleCounter(ref ulong cnt, ref byte ovf)
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetCycleCounter(ref cnt, ref ovf);
        }
        return num;
    }

    public int m_SetCycleCounter(ulong cnt)
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_SetCycleCounter(cnt);
        }
        return num;
    }

    public int m_ReadIEMemory(uint adrs, uint len, byte[] val)
    {
        if (len == 0)
        {
            return -1;
        }
        int num = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        uint num2 = adrs + len - 1;
        for (uint num3 = adrs; num3 <= num2; num3++)
        {
            if (num3 < m_CodeStartAdr || num3 > m_CodeEndAdr)
            {
                return 2;
            }
            byte b = 0;
            int num4 = CSimDbgApp.dbgApi_GetIEVal(num3, ref b);
            val[num] = b;
            if (num4 != 0)
            {
                return num4;
            }
            num++;
        }
        return 0;
    }

    public int m_SetMemoryModel(byte model)
    {
        if (model != 0 && model != 1)
        {
            return 16;
        }
        m_MemoryModel = model;
        return 0;
    }

    public int m_ReadRegAll(uint[] val)
    {
        int result = 0;
        if (!m_Initflag)
        {
            InitSetting();
        }
        for (int i = 0; i < 32; i++)
        {
            val[i] = (uint)m_Reg.GetReg(BM.I2B(i));
        }
        return result;
    }

    public bool checkDM(int n, bool saveflg = false)
    {
        BRKPARAM bRKPARAM = default(BRKPARAM);
        bRKPARAM.InitDMParam();
        bool result = false;
        CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
        if ((n & 1) == 1)
        {
            if ((bRKPARAM.dm_param[n].access & 1) == 0)
            {
                if (RdRamCnt == 1)
                {
                    RdRamAdr[1] = RdRamAdr[0] + 1;
                    byte[] array = new byte[1];
                    m_ReadDataMemory(RdRamAdr[1], 1u, array);
                    RdRamDat[1] = array[0];
                }
                for (int i = 0; i < RdRamCnt; i += 2)
                {
                    if (bRKPARAM.dm_param[n].ramadrs != (RdRamAdr[i + 1] & bRKPARAM.dm_param[n].ramadrsmask) || BM.I2W((bRKPARAM.dm_param[n].ramdata << 8) + bRKPARAM.dm_param[n - 1].ramdata) != ((RdRamAdr[i] >= RdRamAdr[i + 1]) ? BM.I2W(((RdRamDat[i] & bRKPARAM.dm_param[n].ramdatamask) << 8) + (RdRamDat[i + 1] & bRKPARAM.dm_param[n - 1].ramdatamask)) : BM.I2W(((RdRamDat[i + 1] & bRKPARAM.dm_param[n].ramdatamask) << 8) + (RdRamDat[i] & bRKPARAM.dm_param[n - 1].ramdatamask))))
                    {
                        continue;
                    }
                    result = true;
                    if (saveflg)
                    {
                        m_DMadrs = RdRamAdr[i];
                        if (RdRamAdr[i] < RdRamAdr[i + 1])
                        {
                            m_DMwval = BM.I2W(RdRamDat[i] + (RdRamDat[i + 1] << 8));
                        }
                        else
                        {
                            m_DMwval = BM.I2W((RdRamDat[i] << 8) + RdRamDat[i + 1]);
                        }
                    }
                }
            }
            if (bRKPARAM.dm_param[n].access > 0)
            {
                if (TrcRamCnt == 1)
                {
                    TrcRamAdr[1] = TrcRamAdr[0] + 1;
                    byte[] array2 = new byte[1];
                    m_ReadDataMemory(TrcRamAdr[1], 1u, array2);
                    TrcRamDat[1] = array2[0];
                }
                for (int i = 0; i < TrcRamCnt; i += 2)
                {
                    if (bRKPARAM.dm_param[n].ramadrs != (TrcRamAdr[i + 1] & bRKPARAM.dm_param[n].ramadrsmask) || BM.I2W((bRKPARAM.dm_param[n].ramdata << 8) + bRKPARAM.dm_param[n - 1].ramdata) != ((TrcRamAdr[i] >= TrcRamAdr[i + 1]) ? BM.I2W(((TrcRamDat[i] & bRKPARAM.dm_param[n].ramdatamask) << 8) + (TrcRamDat[i + 1] & bRKPARAM.dm_param[n - 1].ramdatamask)) : BM.I2W(((TrcRamDat[i + 1] & bRKPARAM.dm_param[n].ramdatamask) << 8) + (TrcRamDat[i] & bRKPARAM.dm_param[n - 1].ramdatamask))))
                    {
                        continue;
                    }
                    result = true;
                    if (saveflg)
                    {
                        m_DMadrs = TrcRamAdr[i];
                        if (TrcRamAdr[i] < TrcRamAdr[i + 1])
                        {
                            m_DMwval = BM.I2W(TrcRamDat[i] + (TrcRamDat[i + 1] << 8));
                        }
                        else
                        {
                            m_DMwval = BM.I2W((TrcRamDat[i] << 8) + TrcRamDat[i + 1]);
                        }
                    }
                }
            }
        }
        else
        {
            if ((bRKPARAM.dm_param[n].access & 1) == 0)
            {
                for (int i = 0; i < RdRamCnt; i++)
                {
                    if (bRKPARAM.dm_param[n].ramadrs != (RdRamAdr[i] & bRKPARAM.dm_param[n].ramadrsmask))
                    {
                        continue;
                    }
                    if (bRKPARAM.dm_param[n].condition != 0)
                    {
                        if (bRKPARAM.dm_param[n].ramdata == (RdRamDat[i] & bRKPARAM.dm_param[n].ramdatamask))
                        {
                            result = true;
                            if (saveflg)
                            {
                                m_DMadrs = RdRamAdr[i];
                                m_DMbval = RdRamDat[i];
                            }
                        }
                    }
                    else if (bRKPARAM.dm_param[n].ramdata != (RdRamDat[i] & bRKPARAM.dm_param[n].ramdatamask))
                    {
                        result = true;
                        if (saveflg)
                        {
                            m_DMadrs = RdRamAdr[i];
                            m_DMbval = RdRamDat[i];
                        }
                    }
                }
            }
            if (bRKPARAM.dm_param[n].access > 0)
            {
                for (int i = 0; i < TrcRamCnt; i++)
                {
                    if (bRKPARAM.dm_param[n].ramadrs != (TrcRamAdr[i] & bRKPARAM.dm_param[n].ramadrsmask))
                    {
                        continue;
                    }
                    if (bRKPARAM.dm_param[n].condition != 0)
                    {
                        if (bRKPARAM.dm_param[n].ramdata == (TrcRamDat[i] & bRKPARAM.dm_param[n].ramdatamask))
                        {
                            result = true;
                            if (saveflg)
                            {
                                m_DMadrs = TrcRamAdr[i];
                                m_DMbval = TrcRamDat[i];
                            }
                        }
                    }
                    else if (bRKPARAM.dm_param[n].ramdata != (TrcRamDat[i] & bRKPARAM.dm_param[n].ramdatamask))
                    {
                        result = true;
                        if (saveflg)
                        {
                            m_DMadrs = TrcRamAdr[i];
                            m_DMbval = TrcRamDat[i];
                        }
                    }
                }
            }
        }
        return result;
    }

    public int m_GetIEIncCount(ref uint cnt)
    {
        int num = 0;
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetIEIncCount(ref cnt);
        }
        return num;
    }

    public int m_SetIECntAddress(uint start, uint end, ref uint cnt)
    {
        uint num = 0u;
        byte b = 0;
        if (start < m_CodeStartAdr)
        {
            start = m_CodeStartAdr;
        }
        if (end > m_CodeEndAdr)
        {
            end = m_CodeEndAdr;
        }
        m_CvCntAddressStart = start;
        m_CvCntAddressEnd = end;
        for (uint num2 = start; num2 <= end; num2++)
        {
            CSimDbgApp.dbgApi_GetIEVal(num2, ref b);
            if (b == 1)
            {
                num++;
            }
        }
        cnt = num;
        return 0;
    }

    public int m_SetCoreRevision(string pRev)
    {
        switch (pRev)
        {
            case "A2":
                m_core_rev_type = 0u;
                break;
            case "A3":
            case "A3.0":
            case "A3.1":
            case "A3.2":
                m_core_rev_type = 1u;
                break;
            case "A3.3":
            case "A3.4":
                m_core_rev_type = 2u;
                break;
            case "A3.5":
                m_core_rev_type = 3u;
                break;
            default:
                m_core_rev_type = 3u;
                break;
        }
        return 0;
    }

    public int m_GetCoreRevision(ref uint type)
    {
        type = m_core_rev_type;
        return 0;
    }

    public int m_SetInterruptInfo(byte intnum, ref INTERRUPTTABLE[] intinfo)
    {
        m_InterruptNum = intnum;
        for (int i = 0; i < intnum; i++)
        {
            m_InterruptInfo[i] = intinfo[i];
        }
        InitSfr();
        return 0;
    }

    public int m_SetInterruptTable(byte intnum, ref SIMU8_INTERRUPT_TABLE[] intinfo)
    {
        int num = 0;
        if (64 < intnum)
        {
            return 17;
        }
        for (num = 0; num < intnum; num++)
        {
            if (intinfo[num].vector_adrs < 0 || 255 < intinfo[num].vector_adrs)
            {
                return 18;
            }
            if (intinfo[num].ie_adrs != 0 && (intinfo[num].ie_adrs < 61440 || 65535 < intinfo[num].ie_adrs))
            {
                return 19;
            }
            if (intinfo[num].irq_adrs < 61440 || 65535 < intinfo[num].irq_adrs)
            {
                return 20;
            }
            if (intinfo[num].ie_bit < 0 || 7 < intinfo[num].ie_bit)
            {
                return 21;
            }
            if (intinfo[num].irq_bit < 0 || 7 < intinfo[num].irq_bit)
            {
                return 22;
            }
            int num2 = 0;
            for (int i = 0; i < 20 && intinfo[num].intname[i] != 0; i++)
            {
                num2++;
            }
            if (num2 > 19)
            {
                return 23;
            }
        }
        m_InterruptNum = intnum;
        for (num = 0; num < 64; num++)
        {
            if (num < intnum)
            {
                m_InterruptInfo[num].vector_adrs = intinfo[num].vector_adrs;
                m_InterruptInfo[num].ie_adrs = intinfo[num].ie_adrs;
                m_InterruptInfo[num].ie_bit = intinfo[num].ie_bit;
                m_InterruptInfo[num].ie_mask = (byte)(1 << (int)intinfo[num].ie_bit);
                m_InterruptInfo[num].irq_adrs = intinfo[num].irq_adrs;
                m_InterruptInfo[num].irq_bit = intinfo[num].irq_bit;
                m_InterruptInfo[num].irq_mask = (byte)(1 << (int)intinfo[num].irq_bit);
                m_InterruptInfo[num].InitIntname();
                LIBC.strcpy_s(m_InterruptInfo[num].intname, intinfo[num].intname);
            }
            else
            {
                m_InterruptInfo[num].vector_adrs = 0u;
                m_InterruptInfo[num].ie_adrs = 0u;
                m_InterruptInfo[num].ie_bit = 0;
                m_InterruptInfo[num].ie_mask = 0;
                m_InterruptInfo[num].irq_adrs = 0u;
                m_InterruptInfo[num].irq_bit = 0;
                m_InterruptInfo[num].irq_mask = 0;
                m_InterruptInfo[num].InitIntname();
            }
        }
        if (m_Logflag == 1)
        {
            COutMod cOutMod = new COutMod();
            cOutMod.W(" Interrupt Table:").endl();
            for (num = 0; num < intnum; num++)
            {
                uint v = (uint)(num + 1);
                uint vector_adrs = m_InterruptInfo[num].vector_adrs;
                uint ie_adrs = m_InterruptInfo[num].ie_adrs;
                uint ie_bit = m_InterruptInfo[num].ie_bit;
                uint irq_adrs = m_InterruptInfo[num].irq_adrs;
                uint irq_bit = m_InterruptInfo[num].irq_bit;
                string @string = Encoding.ASCII.GetString(m_InterruptInfo[num].intname);
                cOutMod.dec(v).W(".").W(" Vector: ")
                    .hex4dig(vector_adrs)
                    .W(" IE: ")
                    .hex4dig(ie_adrs)
                    .W(".")
                    .dec(ie_bit)
                    .W("  IRQ: ")
                    .hex4dig(irq_adrs)
                    .W(".")
                    .dec(irq_bit)
                    .W("  Name: ")
                    .W(@string)
                    .endl();
            }
        }
        InitSfr();
        return 0;
    }

    public int m_SetInterruptSetting(INTERRUPTSETTING[] intsetting)
    {
        for (int i = 0; i < m_InterruptNum; i++)
        {
            m_InterruptSetting[i] = intsetting[i];
        }
        m_InitAutoInterruptCounter();
        return 0;
    }

    public int m_GetInterruptSetting(INTERRUPTSETTING[] intsetting)
    {
        for (int i = 0; i < m_InterruptNum; i++)
        {
            intsetting[i] = m_InterruptSetting[i];
        }
        return 0;
    }

    public void m_SetDSR(byte dsr)
    {
        m_DSR = dsr;
        CSimMemApp.memApi_SetVal(12305, 61440, dsr);
    }

    public int m_SetTargetName(string tname)
    {
        m_initPeriData();
        m_trg_name = tname;
        int num = m_readIOfile();
        if (num == 0)
        {
            m_DynamicLink();
            return num;
        }
        m_trg_name = "_default";
        return num;
    }

    public string m_GetTargetName()
    {
        return m_trg_name;
    }

    public byte CheckCodeMapping(uint adrs)
    {
        uint num = 0u;
        uint num2 = 0u;
        byte b = 0;
        byte result = 0;
        ushort num3 = (ushort)((adrs <= 65535) ? 12289 : 12321);
        CSimMemApp.memApi_GetCount(num3, ref b);
        for (int i = 0; i < b; i++)
        {
            CSimMemApp.memApi_GetStartAddress(num3, (byte)i, ref num);
            CSimMemApp.memApi_GetEndAddress(num3, (byte)i, ref num2);
            if (adrs >= num && adrs <= num2)
            {
                CSimMemApp.memApi_GetAttribute(num3, (byte)i, ref result);
                break;
            }
        }
        return result;
    }

    public byte CheckDataMapping(uint adrs)
    {
        uint num = 0u;
        uint num2 = 0u;
        byte b = 0;
        byte result = 0;
        ushort num3 = (ushort)((adrs <= 65535) ? 12305 : 12321);
        CSimMemApp.memApi_GetCount(num3, ref b);
        for (int i = 0; i < b; i++)
        {
            CSimMemApp.memApi_GetStartAddress(num3, (byte)i, ref num);
            CSimMemApp.memApi_GetEndAddress(num3, (byte)i, ref num2);
            if (adrs >= num && adrs <= num2)
            {
                CSimMemApp.memApi_GetAttribute(num3, BM.I2B(i), ref result);
                break;
            }
        }
        return result;
    }

    public void SetAutoIntCount(int i)
    {
        if (m_InterruptSetting[i].maxCycle == 0L)
        {
            m_InterruptAuto[i].repeatCount = m_InterruptSetting[i].minCycle;
            return;
        }
        int num = (int)(m_InterruptSetting[i].maxCycle - m_InterruptSetting[i].minCycle + 1);
        LIBC.srand(m_bfrand);
        int num2 = (m_bfrand = LIBC.rand());
        float num3 = (float)num / 1.0737418E+09f * (float)num2;
        m_InterruptAuto[i].repeatCount = m_InterruptSetting[i].minCycle + (ulong)num3;
    }

    public void InitSfr()
    {
        m_SetDSR(0);
        m_ClearSBYCON();
        for (int i = 0; i < m_InterruptNum; i++)
        {
            CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[i].ie_adrs, 0);
            CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[i].irq_adrs, 0);
        }
    }

    private void m_outputLog_1cyc(int pcbak, ushort code0)
    {
        int[] array = new int[16];
        int insType = m_dasmu8.GetInsType();
        for (int i = 0; i < 16; i++)
        {
            array[i] = m_Reg.GetReg(BM.I2B(i));
        }
        int reg = m_Reg.GetReg(26);
        int reg2 = m_Reg.GetReg(31);
        int reg3 = m_Reg.GetReg(30);
        int reg4 = m_Reg.GetReg(17);
        string text = "    ";
        string text2 = $"{m_gcount:d6} {pcbak:X5} {code0:X4} {text} {reg:X2}  {array[0]:X2}  {array[1]:X2}  {array[2]:X2}  {array[3]:X2}  {array[4]:X2}  {array[5]:X2}  {array[6]:X2}  {array[7]:X2}  {array[8]:X2}  {array[9]:X2}  {array[10]:X2}  {array[11]:X2}  {array[12]:X2}  {array[13]:X2}  {array[14]:X2}  {array[15]:X2}  {reg2:X4} {reg3:X4} {reg4:X4}";
        if (insType == 130 || insType == 131 || insType == 132)
        {
            text2 += " -DSR-";
        }
        text2 += "\n";
        try
        {
            f1.WriteString(text2);
        }
        catch (IOException)
        {
            f1.Abort();
            exit(1);
        }
    }

    public int m_EXproc()
    {
        int result = ExProcFunc(m_ex_state);
        m_ex_state++;
        if (m_state_clr_id)
        {
            m_ex_state = 0;
        }
        if (m_ea_plus_id)
        {
            m_ea_plus_ex = true;
            return result;
        }
        m_ea_plus_ex = false;
        return result;
    }

    public int ExProcFunc(int ex_state)
    {
        int result = 0;
        int soutex = 0;
        int soutex2 = 0;
        int carry = 0;
        byte greg0_num = BM.I2B(fn_Ex_Greg0Sel());
        byte greg1_num = fn_Ex_Greg1Sel();
        byte greg2_num = BM.I2B(fn_Ex_Greg2Sel(greg1_num));
        int val = BM.UI2I(fn_Ex_CBus0Sel(greg0_num));
        int num = BM.UI2I(fn_Ex_CBus1Sel(greg0_num, greg1_num));
        int abus_data = BM.UI2I(fn_Ex_ABusSel(num, BM.UI2I(fn_Ex_CBus2Sel(greg2_num))));
        int num2 = fn_Ex_EABusSel();
        uint val2 = (m_sel_ex1_cbus0_id ? BM.I2UI(val) : (m_wr_ex1_wb_id ? BM.I2UI(m_WB) : 0u));
        uint val3 = (m_sel_ex2_cbus1_id ? BM.I2UI(num) : 0u);
        int num3 = fn_Ex_ADD16(fn_Ex_A16LSel(num2), fn_Ex_A16RSel(abus_data));
        if (m_wr_arh_id || m_wr_arl_id)
        {
            if (m_sel_arbus_eabus_id)
            {
                m_AR = num2;
            }
            else
            {
                m_AR = num3;
            }
        }
        if (m_wr_eah_id || m_wr_eal_id)
        {
            m_WriteReg(31, BM.I2UI(num3));
        }
        if (m_wr_sp_id)
        {
            if (m_inst_mode == 1)
            {
                num3 &= 0xFFFE;
            }
            m_WriteReg(30, BM.I2UI(num3));
        }
        fn_Ex_Shifter(BM.UI2I(val2), BM.UI2I(val3), m_AR, ref soutex, ref soutex2, ref carry);
        int num4 = fn_Ex_ALU(soutex, soutex2);
        if (m_sel_abus_bound_id)
        {
            m_AR = BM.L2I(m_AR & 0xFFFFFFFEu);
        }
        int arg_dtadr = m_AR & 0xFFFF;
        int arg_dtwdata = num4;
        m_WB = num4;
        if (m_edsr_id)
        {
            m_edsr_ex = true;
        }
        fn_Ex_Update_MemReg(arg_dtadr, arg_dtwdata, greg0_num, greg1_num);
        if (m_brkswi_id)
        {
            byte pSW = m_Reg.GetPSW(54);
            switch (m_ex_state)
            {
                case 2:
                    if (pSW >= 2)
                    {
                        m_next_next_pc = 0u;
                    }
                    else
                    {
                        m_next_next_pc = 4u;
                    }
                    break;
                case 4:
                    m_next_next_pc = 2u;
                    break;
                case 5:
                    m_Reg.Reset();
                    break;
            }
        }
        if (m_sel_csr_cbus0_id)
        {
            m_WriteReg(21, BM.I2UI(val));
            m_PcChange();
        }
        if (m_sel_pc_pcbus_id || (m_bcc_id && m_bcctrue))
        {
            m_WriteReg(16, BM.I2UI(num3));
            m_PcChange();
        }
        if (m_sel_csr_clr_id)
        {
            m_WriteReg(21, 0u);
        }
        if (m_sel_csr_irn_id)
        {
            m_WriteReg(21, BM.I2UI((m_saveIR >> 8) & 0xF));
            m_PcChange();
        }
        if (m_step_inst1_flag && m_dend_ex)
        {
            m_StepInst = 1;
        }
        else if (m_step_inst2_flag && m_dend_ex)
        {
            m_StepInst = 2;
        }
        else
        {
            m_StepInst = 0;
        }
        return result;
    }

    public int fn_Ex_Greg0Sel()
    {
        int num = 0;
        num = (m_saveIR >> 8) & 0xF;
        if (m_greg0_entry_id)
        {
            num = (m_saveIR >> 8) & 0xF;
        }
        if (m_cop_store_id || (m_cop_read_id && !m_alu_mov_flag))
        {
            num += 32;
        }
        if (m_sel_greg0_regn_bit0or_id)
        {
            num |= 1;
        }
        if (m_sel_greg0_regn_bit1or_id)
        {
            num |= 2;
        }
        if (m_sel_greg0_regn_bit2or_id)
        {
            num |= 4;
        }
        return num;
    }

    public byte fn_Ex_Greg1Sel()
    {
        int num = 0;
        num = (m_saveIR >> 4) & 0xF;
        if (m_greg1_entry_id)
        {
            num = ((!m_shift_id) ? ((m_saveIR >> 4) & 0xF) : ((!m_right_id) ? (((m_saveIR >> 8) - 1) & 0xF) : (((m_saveIR >> 8) + 1) & 0xF)));
        }
        if (m_sel_greg0_bp_id)
        {
            num = 12;
        }
        if (m_sel_greg0_fp_id)
        {
            num = 14;
        }
        if (m_cop_read_id && m_alu_mov_flag)
        {
            num += 32;
        }
        if (m_sel_greg1_regm_bit0or_id)
        {
            num |= 1;
        }
        return BM.I2B(num);
    }

    public int fn_Ex_Greg2Sel(byte greg1_num)
    {
        int num = 0;
        num = greg1_num | 1;
        if (m_greg2_entry_id)
        {
            num = ((!m_shift_id) ? (((m_saveIR >> 4) & 0xF) | 1) : ((m_saveIR >> 4) & 0xF));
        }
        return num;
    }

    public uint fn_Ex_CBus0Sel(byte greg0_num)
    {
        int num = 0;
        uint val = 0u;
        if (m_neg_id)
        {
            val = 0u;
        }
        else if (m_sel_r0_clrh)
        {
            m_ReadReg(17, ref val);
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_r0_clrl)
        {
            m_ReadReg(17, ref val);
        }
        else if (m_sel_r0_eah)
        {
            m_ReadReg(31, ref val);
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_r0_eal)
        {
            m_ReadReg(31, ref val);
        }
        else if (m_sel_r0_ecsr)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(22, ref val);
                    break;
                case 1:
                    m_ReadReg(23, ref val);
                    break;
                case 2:
                    m_ReadReg(24, ref val);
                    break;
                case 3:
                    m_ReadReg(25, ref val);
                    break;
            }
        }
        else if (m_sel_r0_elrh)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(17, ref val);
                    break;
                case 1:
                    m_ReadReg(18, ref val);
                    break;
                case 2:
                    m_ReadReg(19, ref val);
                    break;
                case 3:
                    m_ReadReg(20, ref val);
                    break;
            }
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_r0_elrl)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(17, ref val);
                    break;
                case 1:
                    m_ReadReg(18, ref val);
                    break;
                case 2:
                    m_ReadReg(19, ref val);
                    break;
                case 3:
                    m_ReadReg(20, ref val);
                    break;
            }
        }
        else if (m_sel_r0_epsw)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    val = 255u;
                    break;
                case 1:
                    m_ReadReg(27, ref val);
                    break;
                case 2:
                    m_ReadReg(28, ref val);
                    break;
                case 3:
                    m_ReadReg(29, ref val);
                    break;
            }
        }
        else if (m_sel_r0_irl)
        {
            val = BM.I2UI((m_saveIR >> 4) & 0xF);
        }
        else if (m_sel_r0_lcsr)
        {
            m_ReadReg(22, ref val);
        }
        else if (m_sel_r0_mul_div)
        {
            val = BM.I2UI(m_mul_div_ex1);
        }
        else if (m_sel_r0_psw)
        {
            m_ReadReg(26, ref val);
        }
        else
        {
            m_ReadReg(greg0_num, ref val);
            if (m_dtword_id)
            {
                uint val2 = 0u;
                m_ReadReg(BM.I2B(greg0_num + 1), ref val2);
                val = ((val2 << 8) & 0xFF00) + (val & 0xFF);
            }
        }
        return val;
    }

    public uint fn_Ex_CBus1Sel(byte greg0_num, byte greg1_num)
    {
        int num = 0;
        uint val = 0u;
        if (m_neg_id)
        {
            m_ReadReg(greg0_num, ref val);
        }
        else if (m_wr_disp6_id)
        {
            val = BM.I2UI(((m_saveIR & 0x20) << 2) + ((m_saveIR & 0x20) << 1) + (m_saveIR & 0x3F));
        }
        else if (m_sel_cbus1_bcc_id)
        {
            val = BM.I2UI(m_saveIR & 0xFF);
        }
        else if (m_sel_cbus1_borrow_id)
        {
            val = BM.I2UI(-m_borrow);
        }
        else if (m_sel_cbus1_clrl_id)
        {
            m_ReadReg(17, ref val);
        }
        else if (m_sel_cbus1_const1_id)
        {
            val = 1u;
        }
        else if (m_sel_cbus1_daa_id)
        {
            val = 102u;
        }
        else if (m_sel_cbus1_ecsr_id)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(22, ref val);
                    break;
                case 1:
                    m_ReadReg(23, ref val);
                    break;
                case 2:
                    m_ReadReg(24, ref val);
                    break;
                case 3:
                    m_ReadReg(25, ref val);
                    break;
            }
        }
        else if (m_sel_cbus1_elrh_id)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(17, ref val);
                    break;
                case 1:
                    m_ReadReg(18, ref val);
                    break;
                case 2:
                    m_ReadReg(19, ref val);
                    break;
                case 3:
                    m_ReadReg(20, ref val);
                    break;
            }
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_cbus1_elrl_id)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(17, ref val);
                    break;
                case 1:
                    m_ReadReg(18, ref val);
                    break;
                case 2:
                    m_ReadReg(19, ref val);
                    break;
                case 3:
                    m_ReadReg(20, ref val);
                    break;
            }
        }
        else if (m_sel_cbus1_epsw_id)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(26, ref val);
                    break;
                case 1:
                    m_ReadReg(27, ref val);
                    break;
                case 2:
                    m_ReadReg(28, ref val);
                    break;
                case 3:
                    m_ReadReg(29, ref val);
                    break;
            }
        }
        else if (m_sel_cbus1_irl_id)
        {
            val = BM.I2UI(m_saveIR & 0xFF);
        }
        else if (m_sel_cbus1_mul_div_ex2_id)
        {
            val = BM.I2UI(m_mul_div_ex2);
        }
        else if (m_sel_cbus1_psw_id)
        {
            m_ReadReg(26, ref val);
        }
        else if (m_sel_cbus1_roml_id)
        {
            val = m_pcrdata;
        }
        else if (m_sel_cbus1_sph_id)
        {
            m_ReadReg(30, ref val);
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_cbus1_spl_id)
        {
            m_ReadReg(30, ref val);
        }
        else if (m_sel_cbus1_wbdata_id)
        {
            val = BM.I2UI(m_WB);
        }
        else
        {
            m_ReadReg(greg1_num, ref val);
            if (m_dtword_id)
            {
                uint val2 = 0u;
                m_ReadReg(BM.I2B(greg1_num + 1), ref val2);
                val = BM.MAKEWORD((byte)val, (byte)val2);
            }
        }
        return val;
    }

    public uint fn_Ex_CBus2Sel(byte greg2_num)
    {
        int num = 0;
        uint val = 0u;
        if (m_sel_cbus2_clrh_id)
        {
            m_ReadReg(17, ref val);
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_cbus2_elrh_id)
        {
            switch (m_Reg.GetPSW(54))
            {
                case 0:
                    m_ReadReg(17, ref val);
                    break;
                case 1:
                    m_ReadReg(18, ref val);
                    break;
                case 2:
                    m_ReadReg(19, ref val);
                    break;
                case 3:
                    m_ReadReg(20, ref val);
                    break;
            }
            val = (val >> 8) & 0xFFu;
        }
        else if (m_sel_cbus2_irl_id)
        {
            val = BM.I2UI(m_saveIR & 0xFF);
        }
        else if (m_sel_cbus2_romh_id)
        {
            val = BM.I2UI((m_pcrdata >> 8) & 0xFF);
        }
        else if (m_sel_cbus2_wbdata_id)
        {
            val = BM.I2UI(m_WB);
        }
        else if (m_wr_disp6_id)
        {
            val = (((m_saveIR & 0x20u) != 0) ? 65535u : 0u);
        }
        else
        {
            m_ReadReg(greg2_num, ref val);
            if (m_dtword_id)
            {
                uint val2 = 0u;
                m_ReadReg(BM.I2B(greg2_num + 1), ref val2);
                val = ((val2 << 8) & 0xFF00) + (val & 0xFF);
            }
        }
        return val;
    }

    public uint fn_Ex_ABusSel(int cbus1_data, int cbus2_data)
    {
        uint num = 0u;
        if (!m_sel_abus_swap_id)
        {
            num = ((!m_sel_abus_width_id) ? BM.I2UI(((cbus2_data & 0xFF) << 8) + (cbus1_data & 0xFF)) : BM.I2UI((cbus2_data & 0x70) >> 4));
        }
        else
        {
            num = BM.I2UI(((cbus1_data & 0x7F) << 9) + ((cbus2_data << 1) & 0xFF));
            if (m_bcc_id)
            {
                num = (num >> 8) & 0xFFu;
                if (((uint)cbus1_data & 0x80u) != 0)
                {
                    num |= 0xFFFFFF00u;
                }
            }
            if (m_shift_id)
            {
                num = (num & 0xFF) >> 1;
            }
        }
        return num;
    }

    public int fn_Ex_EABusSel()
    {
        uint val = 0u;
        if (m_sel_eabus_sp_id)
        {
            m_ReadReg(30, ref val);
        }
        else if (m_sel_adbus_ea_id)
        {
            m_ReadReg(31, ref val);
        }
        else
        {
            val = BM.I2UI(m_AR);
        }
        return BM.UI2I(val);
    }

    public int fn_Ex_A16LSel(int eabus_data)
    {
        uint num = 0u;
        return BM.UI2I(m_sel_a16l_pc_id ? m_next_pc : (m_sel_a16l_eabus_id ? BM.I2UI(eabus_data) : 0u));
    }

    public int fn_Ex_A16RSel(int abus_data)
    {
        uint num = 0u;
        if (m_sel_a16r_1_id)
        {
            num = 1u;
            if (m_dtword_id)
            {
                num *= 2;
            }
        }
        else if (!m_sel_a16r_ff_id)
        {
            num = (m_sel_a16r_abus_id ? BM.I2UI(abus_data) : 0u);
        }
        else
        {
            num = uint.MaxValue;
            if (m_dtword_id)
            {
                num *= 2;
            }
        }
        return BM.UI2I(num);
    }

    public int fn_Ex_ADD16(int a16l_data, int a16r_data)
    {
        uint num = 0u;
        if (m_alu_adsp_flag)
        {
            a16r_data = (sbyte)(a16r_data & 0xFF);
            num = BM.I2UI(a16l_data + a16r_data);
        }
        else
        {
            num = BM.I2UI(a16l_data + a16r_data);
        }
        return BM.UI2I(num);
    }

    public int fn_Ex_Shifter(int ex1_data, int ex2_data, int AR_data, ref int soutex1, ref int soutex2, ref int carry)
    {
        int num = 0;
        uint val = 0u;
        if (0 == 0)
        {
            soutex1 = ex1_data;
            soutex2 = ex2_data;
            carry = 0;
            m_ReadReg(26, ref val);
            val = (val >> 7) & 1u;
            if (m_shift_imm7_flag)
            {
                ex2_data = (((ex2_data & 0x40) == 0) ? (ex2_data & 0x3F) : BM.L2I(ex2_data | 0xFFFFFFC0u));
                soutex1 = ex1_data;
                soutex2 = ex2_data;
            }
            if (m_shift_signextend_flag || m_shift_extend_flag)
            {
                ex2_data = (((ex2_data & 0x80) == 0) ? (ex2_data & 0x7F) : BM.L2I(ex2_data | 0xFFFFFF80u));
                soutex1 = (ex2_data >> 8) & 0xFF;
                soutex2 = ex2_data & 0xFF;
                if (m_shift_signextend_flag)
                {
                    if (soutex2 == 0)
                    {
                        m_Reg.SetPSW(49, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(49, 0);
                    }
                    m_Reg.SetPSW(50, BM.I2B((soutex2 & 0x80) >> 7));
                }
            }
            if (m_shift_sra_flag)
            {
                num = BM.L2I(((ex2_data << 16) & 0xFF0000) + ((ex1_data << 8) & 0xFF00) + (val << 7));
                byte num2 = BM.I2B((ex1_data >> 7) & 1);
                num >>= m_AR & 7;
                int num3 = BM.UL2I((uint)(-1 << 16 - (m_AR & 7)));
                num = ((num2 == 0) ? (num & ~num3) : (num | num3));
                soutex1 = (num >> 8) & 0xFF;
                soutex2 = num & 0xFF;
                carry = (soutex2 >> 7) & 1;
                if (m_AR != 0)
                {
                    m_Reg.SetPSW(48, BM.I2B(carry));
                }
            }
            if (m_shift_srlc_flag)
            {
                num = BM.L2I(((ex2_data << 16) & 0xFF0000) + ((ex1_data << 8) & 0xFF00) + (val << 7)) >> (m_AR & 7);
                soutex1 = (num >> 8) & 0xFF;
                soutex2 = num & 0xFF;
                carry = (soutex2 >> 7) & 1;
                if (m_AR != 0)
                {
                    m_Reg.SetPSW(48, BM.I2B(carry));
                }
            }
            if (m_shift_sllc_flag)
            {
                num = BM.L2I(((val << 16) & 0x100) + ((ex1_data << 8) & 0xFF00) + (ex2_data & 0xFF)) << (m_AR & 7);
                soutex1 = (num >> 8) & 0xFF;
                soutex2 = num & 0xFF;
                carry = (num >> 16) & 1;
                if (m_AR != 0)
                {
                    m_Reg.SetPSW(48, BM.I2B(carry));
                }
            }
            if (m_shift_sll_flag)
            {
                num = BM.L2I(((val << 16) & 0x100) + ((ex1_data << 8) & 0xFF00)) << (m_AR & 7);
                soutex1 = (num >> 8) & 0xFF;
                soutex2 = num & 0xFF;
                carry = (num >> 16) & 1;
                if (m_AR != 0)
                {
                    m_Reg.SetPSW(48, BM.I2B(carry));
                }
            }
            if (m_shift_srl_flag)
            {
                num = BM.L2I(((ex1_data << 8) & 0xFF00) + (val << 7)) >> (m_AR & 7);
                soutex1 = (num >> 8) & 0xFF;
                soutex2 = num & 0xFF;
                carry = (soutex2 >> 7) & 1;
                if (m_AR != 0)
                {
                    m_Reg.SetPSW(48, BM.I2B(carry));
                }
            }
        }
        return 0;
    }

    public int fn_Ex_ALU(int soutex1_data, int soutex2_data)
    {
        ushort result = BM.I2W(soutex1_data);
        int num = 0;
        byte pSW = m_Reg.GetPSW(48);
        byte pSW2 = m_Reg.GetPSW(49);
        byte b = 0;
        byte b2 = 0;
        byte b3 = 0;
        byte b4 = 0;
        byte b5 = 0;
        byte b6 = 0;
        byte b7 = 0;
        byte b8 = 0;
        ushort num2 = 0;
        ushort num3 = 0;
        m_borrow = 0;
        if (m_alu_reverse_flag)
        {
            soutex2_data = soutex1_data;
            result = BM.I2W(soutex1_data);
        }
        if (!m_dtword_id && !m_alu_mul_flag && !m_alu_div_flag)
        {
            num2 = BM.I2W((ushort)soutex1_data & 0xFF);
            num3 = ((!m_shift_imm7_flag) ? BM.I2W((ushort)soutex2_data & 0xFF) : BM.I2W(soutex2_data));
        }
        else
        {
            num2 = BM.I2W((ushort)soutex1_data & 0xFFFF);
            num3 = BM.I2W((ushort)soutex2_data & 0xFFFF);
        }
        if (m_alu_cpc_flag || m_alu_sbc_flag)
        {
            num = num2 - (num3 + pSW);
            result = BM.I2W(num & 0xFF);
            b2 = BM.I2B((num < 0) ? 1 : 0);
            m_Reg.SetPSW(48, b2);
            b2 = BM.I2B((pSW2 != 0 && (num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 > 127 && (num & 0x80) == 128) || (num2 > 127 && num3 < 128 && (num & 0x80) == 0)) ? 1 : 0));
            if ((num2 & 0xF) < (num3 & 0xF) + pSW)
            {
                m_Reg.SetPSW(53, 1);
            }
            else
            {
                m_Reg.SetPSW(53, 0);
            }
        }
        if (m_alu_add_flag)
        {
            uint num6;
            if (m_shift_imm7_flag)
            {
                ushort num4;
                ushort num5 = (num4 = num3);
                num = num2 + num4;
                num6 = BM.I2UI((byte)num2 + (byte)num4);
                if ((short)num5 < 0)
                {
                    m_borrow = 1;
                }
            }
            else
            {
                num = num2 + num3;
                num6 = (ushort)(num2 + num3);
            }
            if (!m_dtword_id)
            {
                byte pSW3 = m_Reg.GetPSW(49);
                if (((uint)m_ex_state & (true ? 1u : 0u)) != 0)
                {
                    num = (num & 0xFF) + pSW;
                    num6 = (num6 & 0xFFFF) + pSW;
                }
                b2 = BM.I2B((num6 > 255) ? 1 : 0);
                m_Reg.SetPSW(48, b2);
                if (((num & 0xFF) == 0 && m_ex_state == 0) || ((num & 0xFF) == 0 && pSW3 == 1))
                {
                    m_Reg.SetPSW(49, 1);
                }
                else
                {
                    m_Reg.SetPSW(49, 0);
                }
                b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
                m_Reg.SetPSW(50, b2);
                m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 < 128 && (num & 0x80) == 128) || (num2 > 127 && num3 > 127 && (num & 0x80) == 0)) ? 1 : 0));
                if (m_ex_state == 0)
                {
                    if ((num2 & 0xF) + (num3 & 0xF) > 15)
                    {
                        m_Reg.SetPSW(53, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(53, 0);
                    }
                }
                else if ((num2 & 0xF) + ((num3 & 0xF) + pSW) > 15)
                {
                    m_Reg.SetPSW(53, 1);
                }
                else
                {
                    m_Reg.SetPSW(53, 0);
                }
                result = BM.I2W(num & 0xFF);
            }
            else
            {
                ushort num7 = 0;
                b2 = BM.I2B((num > 65535) ? 1 : 0);
                m_Reg.SetPSW(48, b2);
                b2 = BM.I2B(((num & 0xFFFF) == 0) ? 1 : 0);
                m_Reg.SetPSW(49, b2);
                b2 = BM.I2B(((num & 0x8000) == 32768) ? 1 : 0);
                m_Reg.SetPSW(50, b2);
                m_Reg.SetPSW(51, (byte)(((num2 < 32768 && num3 < 32768 && (num & 0x8000) == 32768) || (num2 > 32767 && num3 > 32767 && (num & 0x8000) == 0)) ? 1 : 0));
                if ((num2 & 0xFFF) + (num3 & 0xFFF) > 4095)
                {
                    m_Reg.SetPSW(53, 1);
                }
                else
                {
                    m_Reg.SetPSW(53, 0);
                }
                result = BM.I2W(num & 0xFFFF);
            }
        }
        if (m_alu_adc_flag)
        {
            num = num2 + num3 + pSW;
            b2 = BM.I2B((num > 255) ? 1 : 0);
            m_Reg.SetPSW(48, b2);
            b2 = BM.I2B((pSW2 == 1 && (num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 < 128 && (num & 0x80) == 128) || (num2 > 127 && num3 > 127 && (num & 0x80) == 0)) ? 1 : 0));
            if ((num2 & 0xF) + (num3 & 0xF) + pSW > 15)
            {
                m_Reg.SetPSW(53, 1);
            }
            else
            {
                m_Reg.SetPSW(53, 0);
            }
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_sub_flag)
        {
            num = num2 - num3;
            b2 = BM.I2B((num < 0) ? 1 : 0);
            m_Reg.SetPSW(48, b2);
            b2 = BM.I2B(((num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 > 127 && (num & 0x80) == 128) || (num2 > 127 && num3 < 128 && (num & 0x80) == 0)) ? 1 : 0));
            if ((num2 & 0xF) < (num3 & 0xF))
            {
                m_Reg.SetPSW(53, 1);
            }
            else
            {
                m_Reg.SetPSW(53, 0);
            }
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_mov_flag)
        {
            if (!m_dtword_id)
            {
                if (m_ex_state != 0 && m_sel_cbus1_irl_id)
                {
                    num3 = BM.I2W(num3 >> 8);
                }
                result = BM.I2W(num3 & 0xFF);
            }
            else
            {
                result = BM.I2W(num3 & 0xFFFF);
            }
            if ((m_saveIR & 0xF11F) == 61445 || (m_saveIR & 0xF180) == 57344)
            {
                if (!m_dtword_id)
                {
                    byte pSW4 = m_Reg.GetPSW(49);
                    if ((num3 == 0 && m_ex_state == 0) || (num3 == 0 && pSW4 == 1))
                    {
                        m_Reg.SetPSW(49, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(49, 0);
                    }
                    b2 = BM.I2B(((num3 & 0x80) == 128) ? 1 : 0);
                    m_Reg.SetPSW(50, b2);
                }
                else
                {
                    if (num3 == 0)
                    {
                        m_Reg.SetPSW(49, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(49, 0);
                    }
                    b2 = BM.I2B(((num3 & 0x8000) == 32768) ? 1 : 0);
                    m_Reg.SetPSW(50, b2);
                }
            }
            if ((m_saveIR & 0xF00F) == 32768 || (m_saveIR & 0xF000) == 0)
            {
                m_Reg.GetPSW(49);
                b2 = BM.I2B((num3 == 0) ? 1 : 0);
                m_Reg.SetPSW(49, b2);
                b2 = BM.I2B(((num3 & 0x80) == 128) ? 1 : 0);
                m_Reg.SetPSW(50, b2);
                result = BM.I2W(num3 & 0xFF);
            }
            if (m_edsr_id)
            {
                m_SetDSR((byte)num3);
            }
        }
        if (m_alu_and_flag)
        {
            num = num2 & num3;
            b2 = BM.I2B((num == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_or_flag)
        {
            num = num2 | num3;
            b2 = BM.I2B((num == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_xor_flag)
        {
            num = num2 ^ num3;
            b2 = BM.I2B((num == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_rb_flag)
        {
            num3 = BM.I2W((num3 >> 4) & 7);
            b3 = BM.I2B(((num2 & (1 << (int)num3)) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b3);
            result = BM.I2W(num2 & BM.I2B(~(1 << (int)num3)) & 0xFF);
        }
        if (m_alu_sb_flag)
        {
            num3 = BM.I2W((num3 >> 4) & 7);
            b3 = BM.I2B(((num2 & (1 << (int)num3)) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b3);
            result = BM.I2W((num2 | (1 << (int)num3)) & 0xFF);
        }
        if (m_alu_daa_flag)
        {
            b7 = BM.I2B(num2 & 0xF);
            b8 = BM.I2B((num2 >> 4) & 0xF);
            b4 = m_Reg.GetPSW(48);
            b6 = m_Reg.GetPSW(53);
            if (b6 == 0 && b7 <= 9)
            {
                if (b4 == 0 && b8 <= 9)
                {
                    b = BM.I2B(num3 & 0);
                }
                else
                {
                    b = BM.I2B(num3 & 0xF0);
                    b4 = 1;
                }
            }
            else if (b6 == 0 && b7 >= 10)
            {
                if (b4 == 0 && b8 <= 8)
                {
                    b = BM.I2B(num3 & 0xF);
                }
                else
                {
                    b = BM.I2B(num3 & 0xFF);
                    b4 = 1;
                }
            }
            else if (b4 == 0 && b8 <= 9)
            {
                b = BM.I2B(num3 & 0xF);
            }
            else
            {
                b = BM.I2B(num3 & 0xFF);
                b4 = 1;
            }
            num = num2 + b;
            m_Reg.SetPSW(48, b4);
            b3 = BM.I2B(((num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b3);
            b5 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b5);
            b6 = BM.I2B((b7 + (b & 0xF) > 15) ? 1 : 0);
            m_Reg.SetPSW(53, b6);
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_das_flag)
        {
            b7 = BM.I2B(num2 & 0xF);
            b8 = BM.I2B(num2 >> 4);
            b4 = m_Reg.GetPSW(48);
            b6 = m_Reg.GetPSW(53);
            b = ((b4 == 0 && b8 <= 9) ? ((b6 != 0 || b7 > 9) ? BM.I2B(num3 & 0xF) : BM.I2B(num3 & 0)) : ((b6 != 0 || b7 > 9) ? BM.I2B(num3 & 0xFF) : BM.I2B(num3 & 0xF0)));
            num = num2 - b;
            if (b4 == 0 && num2 < b)
            {
                b4 = 1;
            }
            m_Reg.SetPSW(48, b4);
            b3 = BM.I2B(((num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b3);
            b5 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b5);
            b6 = BM.I2B((b7 < (b & 0xF)) ? 1 : 0);
            m_Reg.SetPSW(53, b6);
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_mul_flag)
        {
            if (m_ex_state == 0)
            {
                m_tempreg = 0;
                m_mul_div_ex1 = 0;
                m_mul_div_ex2 = 0;
            }
            if (((uint)num3 & (true ? 1u : 0u)) != 0)
            {
                m_tempreg += num2;
            }
            m_mul_div_ex1 = num2 << 1;
            m_mul_div_ex2 = num3 >> 1;
            result = BM.I2W(m_tempreg & 0xFF);
            if (m_sel_greg0_regn_bit0or_id)
            {
                result = BM.I2W((m_tempreg >> 8) & 0xFF);
            }
        }
        if (m_alu_div_flag)
        {
            if (m_ex_state == 0)
            {
                m_tempreg = 0;
                m_mul_div_ex1 = 0;
                m_mul_div_ex2 = 0;
            }
            if (num3 == 0)
            {
                m_Reg.SetPSW(48, 1);
            }
            else
            {
                m_Reg.SetPSW(48, 0);
                m_tempreg <<= 1;
                if ((num2 & 0x80u) != 0)
                {
                    m_tempreg |= 1;
                }
                num2 = BM.I2W(num2 << 1);
                if (m_tempreg >= num3)
                {
                    num2++;
                    m_tempreg -= num3;
                }
            }
            m_mul_div_ex2 = num3;
            result = BM.I2W(num2 & 0xFF);
            m_mul_div_ex1 = num2;
        }
        if (m_alu_cmp_flag)
        {
            byte pSW5 = m_Reg.GetPSW(49);
            num = num2 - num3;
            if (!m_dtword_id)
            {
                pSW5 = m_Reg.GetPSW(49);
                if (((uint)m_ex_state & (true ? 1u : 0u)) != 0)
                {
                    num -= pSW;
                }
                b2 = BM.I2B((num < 0) ? 1 : 0);
                m_Reg.SetPSW(48, b2);
                if (((num & 0xFF) == 0 && m_ex_state == 0) || ((num & 0xFF) == 0 && pSW5 == 1))
                {
                    m_Reg.SetPSW(49, 1);
                }
                else
                {
                    m_Reg.SetPSW(49, 0);
                }
                b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
                m_Reg.SetPSW(50, b2);
                m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 > 127 && (num & 0x80) == 128) || (num2 > 127 && num3 < 128 && (num & 0x80) == 0)) ? 1 : 0));
                if (m_ex_state == 0)
                {
                    if ((num2 & 0xF) < (num3 & 0xF))
                    {
                        m_Reg.SetPSW(53, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(53, 0);
                    }
                }
                else if ((num2 & 0xF) < (num3 & 0xF) + pSW)
                {
                    m_Reg.SetPSW(53, 1);
                }
                else
                {
                    m_Reg.SetPSW(53, 0);
                }
                result = BM.I2W(num & 0xFF);
            }
            else
            {
                b2 = BM.I2B((num < 0) ? 1 : 0);
                m_Reg.SetPSW(48, b2);
                b2 = BM.I2B(((num & 0xFFFF) == 0) ? 1 : 0);
                m_Reg.SetPSW(49, b2);
                b2 = BM.I2B(((num & 0x8000) == 32768) ? 1 : 0);
                m_Reg.SetPSW(50, b2);
                m_Reg.SetPSW(51, (byte)(((num2 < 32768 && num3 > 32767 && (num & 0x8000) == 32768) || (num2 > 32767 && num3 < 32768 && (num & 0x8000) == 0)) ? 1 : 0));
                if ((num2 & 0xFFF) < (num3 & 0xFFF))
                {
                    m_Reg.SetPSW(53, 1);
                }
                else
                {
                    m_Reg.SetPSW(53, 0);
                }
                result = BM.I2W(num & 0xFFFF);
            }
        }
        if (m_alu_psw_or_flag)
        {
            result = BM.I2W(num2 | num3);
        }
        if (m_alu_psw_and_flag)
        {
            result = BM.I2W(num2 & num3);
        }
        if (m_alu_inc_flag)
        {
            num = num2 + num3;
            b2 = BM.I2B((num > 255) ? 1 : 0);
            b2 = BM.I2B(((num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 < 128 && (num & 0x80) == 128) || (num2 > 127 && num3 > 127 && (num & 0x80) == 0)) ? 1 : 0));
            if ((num2 & 0xF) + (num3 & 0xF) > 15)
            {
                m_Reg.SetPSW(53, 1);
            }
            else
            {
                m_Reg.SetPSW(53, 0);
            }
            result = BM.I2W(num & 0xFF);
        }
        if (m_alu_dec_flag)
        {
            num = num2 - num3;
            b2 = BM.I2B((num < 0) ? 1 : 0);
            b2 = BM.I2B(((num & 0xFF) == 0) ? 1 : 0);
            m_Reg.SetPSW(49, b2);
            b2 = BM.I2B(((num & 0x80) == 128) ? 1 : 0);
            m_Reg.SetPSW(50, b2);
            m_Reg.SetPSW(51, (byte)(((num2 < 128 && num3 > 127 && (num & 0x80) == 128) || (num2 > 127 && num3 < 128 && (num & 0x80) == 0)) ? 1 : 0));
            if ((num2 & 0xF) < (num3 & 0xF))
            {
                m_Reg.SetPSW(53, 1);
            }
            else
            {
                m_Reg.SetPSW(53, 0);
            }
            result = BM.I2W(num & 0xFF);
        }
        return result;
    }

    public int fn_Ex_Update_MemReg(int arg_dtadr, int arg_dtwdata, int greg0_num, int greg1_num)
    {
        int result = 0;
        int num = 0;
        uint val = 0u;
        byte buf = 0;
        byte buf2 = 0;
        num = m_Reg.GetPSW(54);
        if (m_cntl_axxx_id)
        {
            if ((m_saveIR & 0xF) == 15)
            {
                m_WriteReg(BM.I2B(22 + num), BM.I2UI(arg_dtwdata));
            }
            else if ((m_saveIR & 0xF) == 12 && num != 0)
            {
                m_WriteReg(BM.I2B(26 + num), BM.I2UI(arg_dtwdata));
            }
        }
        if (m_ea_plus_id)
        {
            m_ReadReg(31, ref val);
            val = ((!m_dtword_id) ? BM.L2UI(val + m_ex_state + 1) : BM.L2UI(val + (m_ex_state + 1) * 2));
            if (m_dtword_id)
            {
                val &= 0xFFFEu;
            }
            else if (m_ex_state > 0)
            {
                val &= 0xFFFEu;
            }
            m_WriteReg(31, val);
        }
        if (m_exe_mul_div_wb_id)
        {
            if (m_sel_temp_id)
            {
                m_WriteReg(BM.I2B(greg1_num), BM.I2UI(m_tempreg));
            }
            else
            {
                m_WriteReg(BM.I2B(greg0_num), BM.I2UI(arg_dtwdata));
            }
        }
        if (m_memory_load_ea_id && !m_cop_read_id)
        {
            ushort num2 = 0;
            int num3 = 0;
            if (m_DSR == 0 || !m_edsr_ex)
            {
                num2 = 12305;
                num3 = arg_dtadr;
            }
            else
            {
                num2 = 12321;
                num3 = arg_dtadr + 65536 * (m_DSR - 1);
            }
            m_edsr_ex_used = true;
            if (num2 == 12305)
            {
                ReadDMemory(BM.I2UI(num3), ref buf);
            }
            else
            {
                CSimMemApp.memApi_GetVal(num2, BM.I2UI(num3), ref buf);
                if (IsRomWinRange_GMem(BM.I2UI(num3 + 65536)))
                {
                    m_rwin_wait_counter++;
                }
                m_rwin_wait_counter += DllMem_wait;
                m_ext_rwinsel = true;
            }
            m_dtrdata = buf;
            m_WB = m_dtrdata;
            RdRamAdr[RdRamCnt] = BM.I2UI((num2 == 12305) ? num3 : (num3 + 65536));
            RdRamDat[RdRamCnt] = BM.I2B(m_dtrdata);
            RdRamCnt++;
            m_TraceLastMemAdr = BM.I2UI((num2 == 12305) ? num3 : (num3 + 65536));
            if (m_dtsize_id)
            {
                m_TraceLastMemDat.byte_h_l = BM.I2B(((m_TraceLastMemAdr & (true ? 1u : 0u)) != 0) ? 1 : 2);
                if (m_TraceLastMemDat.byte_h_l == 1)
                {
                    m_TraceLastMemDat.data = BM.I2UI((m_dtrdata << 8) & 0xFF00);
                }
                else
                {
                    m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFF);
                }
            }
            else
            {
                m_TraceLastMemDat.byte_h_l = 2;
                m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFF);
            }
            if (m_dtword_id)
            {
                if (num2 != 12305)
                {
                    CSimMemApp.memApi_GetVal(num2, BM.I2UI(num3 + 1), ref buf2);
                }
                else
                {
                    ReadDMemory(BM.I2UI(num3 + 1), ref buf2);
                }
                m_dtrdata = ((buf2 << 8) & 0xFF00) + (buf & 0xFF);
                m_WB = m_dtrdata;
                RdRamAdr[RdRamCnt] = BM.I2UI((num2 == 12305) ? (num3 + 1) : (num3 + 1 + 65536));
                RdRamDat[RdRamCnt] = BM.I2B((m_dtrdata >> 8) & 0xFF);
                RdRamCnt++;
                m_TraceLastMemDat.byte_h_l = 0;
                m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFFFF);
            }
            if (m_cop_store_id)
            {
                m_WriteReg(BM.I2B(greg0_num), buf);
                if (m_dtword_id)
                {
                    m_WriteReg(BM.I2B(greg0_num + 1), buf2);
                }
            }
            ushort num4 = 0;
            CSimDbgApp.dbgApi_GetBreakCondition(ref num4);
            if (m_SimRunFlag == 0 && (num4 & 0x20u) != 0)
            {
                if (num2 == 12321)
                {
                    num3 += 65536;
                }
                if (CheckDataMapping(BM.I2UI(num3)) == 0)
                {
                    m_RamNA_brk = 1u;
                }
            }
        }
        if (m_memory_load_id || m_memory_pop_id || m_memory_read_id)
        {
            ushort num5 = 0;
            int num6 = 0;
            if (m_DSR == 0 || !m_edsr_ex)
            {
                num5 = 12305;
                num6 = arg_dtadr;
            }
            else
            {
                num5 = 12321;
                num6 = (m_DSR - 1 << 16) + arg_dtadr;
            }
            m_edsr_ex_used = true;
            if (num5 == 12305)
            {
                ReadDMemory(BM.I2UI(num6), ref buf);
            }
            else
            {
                CSimMemApp.memApi_GetVal(num5, BM.I2UI(num6), ref buf);
                if (IsRomWinRange_GMem(BM.I2UI(num6 + 65536)))
                {
                    m_rwin_wait_counter++;
                }
                m_rwin_wait_counter += DllMem_wait;
                m_ext_rwinsel = true;
            }
            m_dtrdata = buf;
            m_WB = m_dtrdata;
            RdRamAdr[RdRamCnt] = BM.I2UI((num5 == 12305) ? num6 : (num6 + 65536));
            RdRamDat[RdRamCnt] = BM.I2B(m_dtrdata);
            RdRamCnt++;
            m_TraceLastMemAdr = BM.I2UI((num5 == 12305) ? num6 : (num6 + 65536));
            if (m_dtsize_id)
            {
                m_TraceLastMemDat.byte_h_l = BM.I2B(((m_TraceLastMemAdr & (true ? 1u : 0u)) != 0) ? 1 : 2);
                if (m_TraceLastMemDat.byte_h_l == 1)
                {
                    m_TraceLastMemDat.data = BM.I2UI((m_dtrdata << 8) & 0xFF00);
                }
                else
                {
                    m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFF);
                }
            }
            else
            {
                m_TraceLastMemDat.byte_h_l = 2;
                m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFF);
            }
            if (m_memory_load_id || m_memory_pop_id)
            {
                m_WriteReg(BM.I2B(greg0_num), buf);
            }
            if (m_dtword_id && m_dtsize_id)
            {
                if (num5 != 12305)
                {
                    CSimMemApp.memApi_GetVal(num5, BM.I2UI(num6 + 1), ref buf2);
                }
                else
                {
                    ReadDMemory(BM.I2UI(num6 + 1), ref buf2);
                }
                m_dtrdata = (buf2 << 8) + (buf & 0xFF);
                m_WB = m_dtrdata;
                RdRamAdr[RdRamCnt] = BM.I2UI((num5 == 12305) ? (num6 + 1) : (num6 + 1 + 65536));
                RdRamDat[RdRamCnt] = BM.I2B((m_dtrdata >> 8) & 0xFF);
                RdRamCnt++;
                m_TraceLastMemDat.byte_h_l = 0;
                m_TraceLastMemDat.data = BM.I2UI(m_dtrdata & 0xFFFF);
                if (m_memory_load_id || m_memory_pop_id)
                {
                    m_WriteReg(BM.I2B(greg0_num + 1), buf2);
                }
            }
            if (m_memory_load_id)
            {
                if (!m_dtword_id)
                {
                    if (((uint)m_dtrdata & 0x80u) != 0)
                    {
                        m_Reg.SetPSW(50, 1);
                    }
                    else
                    {
                        m_Reg.SetPSW(50, 0);
                    }
                }
                else if (((uint)m_dtrdata & 0x8000u) != 0)
                {
                    m_Reg.SetPSW(50, 1);
                }
                else
                {
                    m_Reg.SetPSW(50, 0);
                }
            }
            ushort num7 = 0;
            CSimDbgApp.dbgApi_GetBreakCondition(ref num7);
            if (m_SimRunFlag == 0 && (num7 & 0x20u) != 0)
            {
                if (num5 == 12321)
                {
                    num6 += 65536;
                }
                if (CheckDataMapping(BM.I2UI(num6)) == 0)
                {
                    m_RamNA_brk = 1u;
                }
            }
        }
        if (m_exe_wr_z_id)
        {
            if (((!m_memory_load_id) ? arg_dtwdata : m_dtrdata) == 0)
            {
                m_Reg.SetPSW(49, 1);
            }
            else
            {
                m_Reg.SetPSW(49, 0);
            }
        }
        if (m_exe_wr_zand_id)
        {
            int num8 = ((!m_memory_load_id) ? arg_dtwdata : m_dtrdata);
            val = m_Reg.GetPSW(49);
            if (val == 1 && num8 == 0)
            {
                m_Reg.SetPSW(49, 1);
            }
            else
            {
                m_Reg.SetPSW(49, 0);
            }
        }
        if (m_memory_store_id)
        {
            ushort num9 = 0;
            int num10 = 0;
            if (m_DSR == 0 || !m_edsr_ex)
            {
                num9 = 12305;
                num10 = arg_dtadr;
            }
            else
            {
                num9 = 12321;
                num10 = (m_DSR - 1 << 16) + arg_dtadr;
            }
            m_edsr_ex_used = true;
            if (m_sel_a16r_ff_id && m_dtword_id && m_dtsize_id)
            {
                if (num9 != 12305)
                {
                    CSimMemApp.memApi_SetVal(num9, BM.I2UI(num10 + 1), BM.I2B((arg_dtwdata >> 8) & 0xFF));
                }
                else
                {
                    WriteDMemory(BM.I2UI(num10 + 1), BM.I2B((arg_dtwdata >> 8) & 0xFF));
                }
                TrcRamAdr[TrcRamCnt] = BM.I2UI((num9 == 12305) ? (num10 + 1) : (num10 + 1 + 65536));
                TrcRamDat[TrcRamCnt] = BM.I2B((arg_dtwdata >> 8) & 0xFF);
                TrcRamCnt++;
            }
            if (num9 == 12305)
            {
                WriteDMemory(BM.I2UI(num10), BM.I2B(arg_dtwdata & 0xFF));
            }
            else
            {
                CSimMemApp.memApi_SetVal(num9, BM.I2UI(num10), BM.I2B(arg_dtwdata & 0xFF));
                if (IsRomWinRange_GMem(BM.I2UI(num10 + 65536)))
                {
                    m_rwin_wait_counter++;
                }
                m_rwin_wait_counter += DllMem_wait;
                m_ext_rwinsel = true;
            }
            if (TrcRamCnt < 16)
            {
                TrcRamAdr[TrcRamCnt] = BM.I2UI((num9 == 12305) ? num10 : (num10 + 65536));
                TrcRamDat[TrcRamCnt] = BM.I2B(arg_dtwdata & 0xFF);
                TrcRamCnt++;
            }
            if (!m_sel_a16r_ff_id && m_dtword_id && m_dtsize_id)
            {
                if (num9 != 12305)
                {
                    CSimMemApp.memApi_SetVal(num9, BM.I2UI(num10 + 1), BM.I2B((arg_dtwdata >> 8) & 0xFF));
                }
                else
                {
                    WriteDMemory(BM.I2UI(num10 + 1), BM.I2B((arg_dtwdata >> 8) & 0xFF));
                }
                if (TrcRamCnt < 16)
                {
                    TrcRamAdr[TrcRamCnt] = BM.I2UI((num9 == 12305) ? (num10 + 1) : (num10 + 1 + 65536));
                    TrcRamDat[TrcRamCnt] = BM.I2B((arg_dtwdata >> 8) & 0xFF);
                    TrcRamCnt++;
                }
            }
            m_TraceLastMemAdr = BM.I2UI((num9 == 12305) ? num10 : (num10 + 65536));
            if (m_dtsize_id)
            {
                if (m_dtword_id)
                {
                    m_TraceLastMemDat.byte_h_l = 0;
                    m_TraceLastMemDat.data = BM.I2UI(arg_dtwdata & 0xFFFF);
                }
                else
                {
                    m_TraceLastMemDat.byte_h_l = BM.I2B(((m_TraceLastMemAdr & (true ? 1u : 0u)) != 0) ? 1 : 2);
                    if (m_TraceLastMemDat.byte_h_l == 1)
                    {
                        m_TraceLastMemDat.data = BM.I2UI((arg_dtwdata << 8) & 0xFF00);
                    }
                    else
                    {
                        m_TraceLastMemDat.data = BM.I2UI(arg_dtwdata & 0xFF);
                    }
                }
            }
            else
            {
                m_TraceLastMemDat.byte_h_l = 2;
                m_TraceLastMemDat.data = BM.I2UI(arg_dtwdata & 0xFF);
            }
            ushort num11 = 0;
            CSimDbgApp.dbgApi_GetBreakCondition(ref num11);
            if (m_SimRunFlag == 0 && (num11 & 0x20u) != 0)
            {
                if (num9 == 12321)
                {
                    num10 += 65536;
                }
                if (CheckDataMapping(BM.I2UI(num10)) == 0)
                {
                    m_RamNA_brk = 1u;
                }
            }
        }
        if (m_pop_eah_id)
        {
            m_edsr_ex_used = true;
            m_WB = m_dtrdata;
            m_ReadReg(31, ref val);
            val = BM.L2UI((val & 0xFF) + ((m_dtrdata << 8) & 0xFF00));
            m_WriteReg(31, val);
        }
        if (m_pop_eal_id)
        {
            m_edsr_ex_used = true;
            m_WB = m_dtrdata;
            m_ReadReg(31, ref val);
            val = (m_dtword_id ? BM.I2UI(m_dtrdata) : BM.L2UI((val & 0xFF00) + (m_dtrdata & 0xFF)));
            m_WriteReg(31, val);
        }
        if (m_tbl_ffef_id && (m_saveIR & 0xF0) == 192)
        {
            val = m_Reg.GetPSW(48);
            if (val != 0)
            {
                m_Reg.SetPSW(48, 0);
            }
            else
            {
                m_Reg.SetPSW(48, 1);
            }
        }
        if (m_wr_csr_wb_id)
        {
            m_WriteReg(21, BM.I2UI(m_WB));
            m_PcChange();
        }
        if (m_wr_ecsr_csr_id || m_wr_intecsr_csr_id)
        {
            m_ReadReg(21, ref val);
            if (m_int_enable)
            {
                if (m_s1_swi_id)
                {
                    m_WriteReg(23, val);
                }
                else
                {
                    m_WriteReg(BM.I2B(22 + m_int_level), val);
                }
                m_retNextPC = BM.L2I(m_retNextPC + (val << 16));
            }
            else
            {
                if (m_s1_swi_id)
                {
                    m_WriteReg(23, val);
                }
                else
                {
                    m_WriteReg(BM.I2B(22 + num), val);
                }
                m_retNextPC = BM.L2I(m_retNextPC + (val << 16));
            }
        }
        if (m_wr_elr_pc_id || m_wr_intelr_pc_id)
        {
            m_ReadReg(16, ref val);
            if (m_int_enable)
            {
                if (m_s1_swi_id)
                {
                    m_WriteReg(18, val + 2);
                    m_retNextPC = BM.UI2I(val + 2);
                }
                else if (m_ex_state == 2)
                {
                    m_WriteReg(BM.I2B(17 + m_int_level), m_next_pc);
                    m_retNextPC = BM.UI2I(m_next_pc);
                }
                else
                {
                    m_WriteReg(BM.I2B(17 + m_int_level), m_pc20);
                    m_retNextPC = BM.UI2I(m_pc20);
                }
            }
            else if (m_s1_swi_id)
            {
                m_WriteReg(18, val + 2);
                m_retNextPC = BM.UI2I(val + 2);
            }
            else
            {
                m_WriteReg(BM.I2B(17 + num), m_next_pc);
                m_retNextPC = BM.UI2I(m_next_pc);
            }
        }
        if (m_wr_elrl_wb_id)
        {
            m_ReadReg(BM.I2B(17 + num), ref val);
            if (!m_dtword_id)
            {
                if (m_ex_state == 0)
                {
                    val = BM.L2UI((val & 0xFF00) + (m_WB & 0xFF));
                    m_WriteReg(BM.I2B(17 + num), val);
                }
                else
                {
                    val = BM.L2UI((val & 0xFF) + ((m_WB << 8) & 0xFF00));
                    m_WriteReg(BM.I2B(17 + num), val);
                }
            }
            else
            {
                m_WriteReg(BM.I2B(17 + num), BM.I2UI(m_WB & 0xFFFF));
            }
        }
        if (m_wr_lcsr_csr_id)
        {
            m_ReadReg(21, ref val);
            m_WriteReg(22, val);
        }
        if (m_wr_lcsr_wb_id)
        {
            m_WriteReg(22, BM.I2UI(m_WB));
        }
        if (m_wr_lr_nextpc_id)
        {
            val = m_next_next_pc;
            m_WriteReg(17, val);
        }
        if (m_wr_lr_pc_id)
        {
            val = m_next_pc;
            m_WriteReg(17, val);
        }
        if (m_wr_lrl_wb_id)
        {
            m_ReadReg(17, ref val);
            if (!m_dtword_id)
            {
                if (m_ex_state == 0)
                {
                    val = BM.L2UI((val & 0xFF00) + (m_WB & 0xFF));
                    m_WriteReg(17, val);
                }
                else
                {
                    val = BM.L2UI((val & 0xFF) + ((m_WB << 8) & 0xFF00));
                    m_WriteReg(17, val);
                }
            }
            else
            {
                m_WriteReg(17, BM.I2UI(m_WB & 0xFFFF));
            }
        }
        if (m_wr_pcl_wb_id)
        {
            m_ReadReg(16, ref val);
            if (!m_dtword_id)
            {
                val = ((((uint)m_ex_state & (true ? 1u : 0u)) != 0) ? BM.L2UI((val & 0xFF) + ((m_WB << 8) & 0xFF00)) : BM.L2UI((val & 0xFF00) + (m_WB & 0xFF)));
                m_WriteReg(16, val);
            }
            else
            {
                m_WriteReg(16, BM.I2UI(m_WB & 0xFFFF));
            }
            m_PcChange();
        }
        if (m_wr_psw_epsw_id)
        {
            m_ReadReg(BM.I2B(27 + (num - 1)), ref val);
            m_WriteReg(26, val);
        }
        if (m_wr_psw_wb_id)
        {
            m_WriteReg(26, BM.I2UI(m_WB));
        }
        if (!m_alu_cmp_flag && !m_alu_cpc_flag && (m_data_move_id || m_exe_rn_rm_id || m_exe_word1_id || m_exe_word3_id))
        {
            if (!m_sel_cbus1_epsw_id || num != 0)
            {
                m_WriteReg(BM.I2B(greg0_num), BM.I2UI(arg_dtwdata & 0xFF));
                if (m_dtword_id)
                {
                    m_WriteReg(BM.I2B(greg0_num + 1), BM.I2UI((arg_dtwdata >> 8) & 0xFF));
                }
            }
            else
            {
                m_WriteReg(BM.I2B(greg0_num), 255u);
            }
        }
        if (m_pc_clear_id)
        {
            m_WriteReg(16, 0u);
            m_PcChange();
        }
        if (m_sel_pc_swivec_id)
        {
            if (m_nmi_req || (m_mi_req && m_Reg.GetPSW(52) != 0) || m_nmice_req)
            {
                m_next_next_pc = m_vector;
            }
            else
            {
                m_next_next_pc = BM.I2UI(128 + (m_IR & 0xFF) * 2);
            }
        }
        if (m_sel_psw_mi_id)
        {
            m_ReadReg(26, ref val);
            m_WriteReg(27, val);
            m_Reg.SetPSW(54, 1);
            m_Reg.SetPSW(52, 0);
        }
        if (m_sel_psw_nmi_id)
        {
            m_ReadReg(26, ref val);
            m_WriteReg(28, val);
            m_Reg.SetPSW(54, 2);
        }
        if (m_sel_psw_nmice_id)
        {
            m_ReadReg(26, ref val);
            m_WriteReg(29, val);
            m_Reg.SetPSW(54, 3);
        }
        if (m_wr_greg_wb_id)
        {
            m_WriteReg(BM.I2B(greg0_num), BM.I2UI(m_WB));
        }
        if (m_edsr_ex_used && m_dend_ex)
        {
            m_edsr_ex = false;
            m_edsr_ex_used = false;
        }
        return result;
    }

    public int m_IDproc()
    {
        m_dtword_id = false;
        m_CheckInterrupt();
        m_InstDecode(m_IR);
        m_id_state++;
        if (m_pipe_hzd)
        {
            m_IdIsEmpty = true;
            m_IfIsEmpty = true;
        }
        else
        {
            m_IdIsEmpty = false;
        }
        return 0;
    }

    public int m_IFproc()
    {
        byte b = 0;
        int num = 0;
        ushort num2 = 0;
        if (num == 0)
        {
            m_ReadReg(21, ref m_csr);
            if (m_csr == 0)
            {
                CSimMemApp.memApi_GetWordVal(12289, m_next_next_pc, ref m_pcrdata);
                b = CheckCodeMapping(m_next_next_pc);
            }
            else
            {
                CSimMemApp.memApi_GetWordVal(12321, m_next_next_pc - 65536, ref m_pcrdata);
                b = CheckCodeMapping(m_next_next_pc);
            }
        }
        if (num == 0)
        {
            CSimDbgApp.dbgApi_GetBreakCondition(ref num2);
            m_RomNA_brk = 0u;
            if ((num2 & 0x10u) != 0 && b == 0)
            {
                m_SimRunFlag = 13;
                m_NextPC = m_pcbak;
                m_BreakPC = m_pcbak;
                m_RomNA_brk = 1u;
                num = 0;
            }
        }
        m_IfIsEmpty = false;
        return num;
    }

    public void m_InstDecode(ushort irdata)
    {
        byte b = (byte)(irdata & 0xFu);
        byte b2 = (byte)((uint)(irdata >> 4) & 0xFu);
        byte b3 = (byte)((uint)(irdata >> 8) & 0xFu);
        byte b4 = (byte)((uint)(irdata >> 12) & 0xFu);
        m_ClearIdSetFlg();
        if ((b4 & 8) == 0)
        {
            exe_rn_imm8(irdata, m_id_state);
            return;
        }
        switch (b4)
        {
            case 8:
                if (b != 15)
                {
                    if ((b & 8) == 0 || (b & 0xE) == 8)
                    {
                        exe_rn_rm(irdata, m_id_state);
                    }
                    else if ((b & 0xE) == 10 || (b & 0xE) == 12 || (b & 0xF) == 14)
                    {
                        shift_rn_rm(irdata, m_id_state);
                    }
                    else
                    {
                        undefine_inst(irdata, m_id_state);
                    }
                    break;
                }
                if ((b2 & 1) == 0)
                {
                    task_extend(irdata, m_id_state);
                    break;
                }
                if ((b2 & 0xD) == 1)
                {
                    task_daa(irdata, m_id_state);
                    break;
                }
                switch (b2)
                {
                    case 5:
                        task_neg(irdata, m_id_state);
                        break;
                    default:
                        if ((b2 & 0xD) != 9 && (b2 & 0xD) != 13)
                        {
                            undefine_inst(irdata, m_id_state);
                            break;
                        }
                        goto case 7;
                    case 7:
                        min_cycle(irdata, m_id_state);
                        if (m_id_state == 0)
                        {
                            m_SimRunFlag = 3;
                        }
                        break;
                }
                break;
            case 9:
                if ((b & 8) == 0)
                {
                    task_lod_st(irdata, m_id_state);
                }
                else if ((b & 0xE) == 8)
                {
                    lst_rn_d16ern(irdata, m_id_state);
                }
                else if ((b & 0xE) == 10 || (b & 0xE) == 12 || (b & 0xF) == 14)
                {
                    shift_rn_imm8(irdata, m_id_state);
                }
                else if ((b & 0xF) == 15)
                {
                    mov_dsr_rn(irdata, m_id_state);
                }
                else
                {
                    undefine_inst(irdata, m_id_state);
                }
                break;
            case 10:
                if (((b & 0xE) == 0 && (b2 & 8) == 0) || (b == 2 && (b2 & 8) == 0))
                {
                    task_sb_rb_tb(irdata, m_id_state);
                    break;
                }
                if (b != 3 && (b & 0xE) != 4)
                {
                    switch (b)
                    {
                        case 7:
                            break;
                        case 6:
                            mov_rn_crm(irdata, m_id_state);
                            return;
                        default:
                            if ((b & 0xE) == 8)
                            {
                                lst_rn_d16ern(irdata, m_id_state);
                                return;
                            }
                            if (b == 10 && (b3 & 1) == 0)
                            {
                                mov_fp_sp(irdata, m_id_state);
                                return;
                            }
                            if (b == 10 && (b3 & 1) == 1)
                            {
                                mov_sp_fp(irdata, m_id_state);
                                return;
                            }
                            if (b != 11 && (b & 0xE) != 12)
                            {
                                switch (b)
                                {
                                    case 15:
                                        break;
                                    case 14:
                                        mov_crn_rm(irdata, m_id_state);
                                        return;
                                    default:
                                        if (((b & 0xE) == 0 && (b2 & 8) == 8) || (b == 2 && (b2 & 8) == 8))
                                        {
                                            task_sb_rb_d16(irdata, m_id_state);
                                        }
                                        else
                                        {
                                            undefine_inst(irdata, m_id_state);
                                        }
                                        return;
                                }
                            }
                            task_mov_cntl_rm(irdata, m_id_state);
                            return;
                    }
                }
                task_mov_rn_cntl(irdata, m_id_state);
                break;
            case 11:
            case 13:
                task_st_rn_disp6(irdata, m_id_state);
                break;
            case 12:
                task_bcc_radr(irdata, m_id_state);
                break;
            case 14:
                if ((b3 & 1) == 0)
                {
                    task_exe_ern_imm7(irdata, m_id_state);
                    break;
                }
                switch (b3)
                {
                    case 1:
                        add_sp_imm8(irdata, m_id_state);
                        break;
                    case 3:
                        mov_dsr_imm8(irdata, m_id_state);
                        break;
                    case 5:
                        swi_imm7(irdata, m_id_state);
                        break;
                    case 7:
                    case 15:
                        min_cycle(irdata, m_id_state);
                        if (m_id_state == 0)
                        {
                            m_SimRunFlag = 3;
                        }
                        break;
                    case 9:
                    case 13:
                        exe_psw_imm8(irdata, m_id_state);
                        break;
                    case 11:
                        if (b == 7 && b2 == 15)
                        {
                            exe_di_com(irdata, m_id_state);
                        }
                        else
                        {
                            exe_psw_imm8(irdata, m_id_state);
                        }
                        break;
                    default:
                        undefine_inst(irdata, m_id_state);
                        break;
                }
                break;
            case 15:
                switch (b)
                {
                    case 0:
                    case 1:
                        task_branch_c16(irdata, m_id_state);
                        break;
                    case 2:
                    case 3:
                        task_branch_ern(irdata, m_id_state);
                        break;
                    case 4:
                        task_mul(irdata, m_id_state);
                        break;
                    case 5:
                    case 6:
                    case 7:
                        task_exe_ern_erm(irdata, m_id_state);
                        break;
                    case 8:
                        min_cycle(irdata, m_id_state);
                        if (m_id_state == 0)
                        {
                            m_SimRunFlag = 3;
                        }
                        break;
                    case 9:
                        task_div(irdata, m_id_state);
                        break;
                    case 10:
                        task_lea_erm(irdata, m_id_state);
                        break;
                    case 11:
                        task_lea_d16_erm(irdata, m_id_state);
                        break;
                    case 12:
                        task_lea_d16(irdata, m_id_state);
                        break;
                    case 13:
                        if ((b2 & 8) == 0)
                        {
                            mov_cprn_ea(irdata, m_id_state);
                        }
                        else if ((b2 & 8) == 8)
                        {
                            mov_ea_cprn(irdata, m_id_state);
                        }
                        else
                        {
                            undefine_inst(irdata, m_id_state);
                        }
                        break;
                    case 14:
                        task_push_pop(irdata, m_id_state);
                        break;
                    case 15:
                        if ((b3 & 8) == 0 || (b3 & 0xE) == 8 || (b3 & 0xE) == 10 || (b3 & 0xE) == 12)
                        {
                            min_cycle(irdata, m_id_state);
                            if (m_id_state == 0)
                            {
                                m_SimRunFlag = 3;
                            }
                            break;
                        }
                        switch (b3)
                        {
                            case 14:
                                switch (b2)
                                {
                                    case 0:
                                    case 1:
                                        m_sel_excom_irm = true;
                                        task_rt_rti(irdata, m_id_state);
                                        break;
                                    case 2:
                                    case 3:
                                        m_sel_excom_irm = true;
                                        inc_dec_ea(irdata, m_id_state);
                                        break;
                                    case 4:
                                    case 5:
                                    case 8:
                                    case 10:
                                    case 11:
                                    case 13:
                                    case 14:
                                        m_sel_excom_irm = true;
                                        min_cycle(irdata, m_id_state);
                                        break;
                                    case 6:
                                    case 7:
                                        m_sel_excom_irm = true;
                                        task_rtice(irdata, m_id_state);
                                        break;
                                    case 9:
                                        m_sel_excom_irm = true;
                                        min_cycle(irdata, m_id_state);
                                        m_edsr_id = true;
                                        m_state_clr_id = true;
                                        m_disint_id = true;
                                        break;
                                    case 12:
                                        m_sel_excom_irm = true;
                                        min_cycle(irdata, m_id_state);
                                        m_tbl_ffef_id = true;
                                        break;
                                    case 15:
                                        m_sel_excom_irm = true;
                                        iceswi(irdata, m_id_state);
                                        break;
                                    default:
                                        undefine_inst(irdata, m_id_state);
                                        break;
                                }
                                break;
                            case 15:
                                table_ffxf(irdata, m_id_state);
                                break;
                            default:
                                undefine_inst(irdata, m_id_state);
                                break;
                        }
                        break;
                    default:
                        undefine_inst(irdata, m_id_state);
                        break;
                }
                break;
            default:
                undefine_inst(irdata, m_id_state);
                break;
        }
    }

    public void m_IRproc_afterCycle(ushort irdata)
    {
        ushort num = irdata;
        bool flag = m_ir_bit11_clr || m_ir_bit10_clr || m_ir_bit9_clr || m_ir_bit8_clr;
        m_Reg.GetPSW(54);
        if (m_bcc_id)
        {
            if (m_bcctrue)
            {
                m_disint_id = true;
                m_saveIR = m_IR;
                return;
            }
            m_ClearIdSetFlg();
            num = 65167;
            min_cycle(num, m_id_state);
            m_IR = irdata;
            m_saveIR = num;
            return;
        }
        if (flag && !m_dend_id)
        {
            byte b = 0;
            _ = m_pcrdata;
            _ = m_pcrdata;
            byte b2 = (byte)((uint)(m_pcrdata >> 8) & 0xFu);
            _ = m_pcrdata;
            b = (m_ir_bit11_clr ? BM.I2B(b2 & 7) : (m_ir_bit10_clr ? BM.I2B(b2 & 0xB) : (m_ir_bit9_clr ? BM.I2B(b2 & 0xD) : ((!m_ir_bit8_clr) ? b2 : BM.I2B(b2 & 0xE)))));
            m_pcrdata = BM.I2W((m_pcrdata & 0xF0FF) | (b << 8));
            m_saveIR = m_IR;
            m_IR = m_pcrdata;
            return;
        }
        if (m_dend_id && (m_IR & 0xFF00) == 58624 && m_int_cycle_id)
        {
            byte b3 = 0;
            CSimMemApp.memApi_GetVal(12305, m_InterruptInfo[m_current_irqno].irq_adrs, ref b3);
            b3 = BM.I2B(b3 & ~m_InterruptInfo[m_current_irqno].irq_mask);
            CSimMemApp.memApi_SetVal(12305, m_InterruptInfo[m_current_irqno].irq_adrs, b3);
            if (m_InterruptAuto[m_current_irqno].onceFlag)
            {
                m_InterruptSetting[m_current_irqno].onceOption = false;
                m_InterruptAuto[m_current_irqno].onceFlag = false;
                if (!m_InterruptSetting[m_current_irqno].repeatOption)
                {
                    m_InterruptSetting[m_current_irqno].autoInterrupt = false;
                }
            }
        }
        m_Int_enable_func();
        if (m_dend_id && m_int_enable)
        {
            m_saveIR = m_IR;
            m_IR = BM.I2W(58624 + m_intnum);
            return;
        }
        if (m_dend_id && !m_int_enable)
        {
            m_saveIR = m_IR;
            m_IR = irdata;
            return;
        }
        if (m_brk_id)
        {
            m_int_enable = true;
            m_int_level = 2;
        }
        m_saveIR = m_IR;
    }

    public void m_Bcctrue_func()
    {
        bool flag = m_Reg.GetPSW(48) == 1;
        bool flag2 = m_Reg.GetPSW(49) == 1;
        bool flag3 = m_Reg.GetPSW(50) == 1;
        bool flag4 = m_Reg.GetPSW(51) == 1;
        bool flag5 = false;
        flag5 = m_excom_out switch
        {
            0 => !flag,
            1 => flag,
            2 => !flag && !flag2,
            3 => flag || flag2,
            4 => flag4 == flag3,
            5 => flag4 ^ flag3,
            6 => !((flag4 ^ flag3) || flag2),
            7 => (flag4 ^ flag3) || flag2,
            8 => !flag2,
            9 => flag2,
            10 => !flag4,
            11 => flag4,
            12 => !flag3,
            13 => flag3,
            14 => true,
            _ => false,
        };
        if (m_bcc_id && flag5)
        {
            m_bcctrue = true;
        }
    }

    public void m_Int_enable_func()
    {
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        bool flag6 = false;
        bool flag7 = false;
        int num = 0;
        int pSW = m_Reg.GetPSW(54);
        if (m_iceswi_id || (m_nmice_req && !m_disint_id && !m_disint_user_id))
        {
            flag = true;
        }
        if (m_edsr_id)
        {
            flag6 = true;
        }
        if (m_disint_id && !flag6)
        {
            flag5 = true;
        }
        if (flag5 || m_disint_user_id)
        {
            flag4 = true;
        }
        if ((m_nmi_req && !flag4) || (m_brk_id && (pSW & 2) == 0))
        {
            flag2 = true;
        }
        if (m_Reg.GetPSW(52) != 0)
        {
            flag7 = true;
        }
        if (m_mi_req && flag7 && !flag4)
        {
            flag3 = true;
        }
        num = (flag ? 3 : (flag2 ? 2 : (flag3 ? 1 : (m_s1_swi_id ? 1 : 0))));
        if (flag || (num >= pSW && (flag2 || flag3 || m_s1_swi_id)))
        {
            m_int_enable = true;
        }
        else
        {
            m_int_enable = false;
        }
        m_int_level = BM.I2B(num);
    }

    public void m_Select_excom()
    {
        byte excom_out = (byte)(m_pcrdata & 0xFu);
        byte excom_out2 = (byte)((uint)(m_pcrdata >> 4) & 0xFu);
        byte excom_out3 = (byte)((uint)(m_pcrdata >> 8) & 0xFu);
        byte excom_out4 = (byte)((uint)(m_pcrdata >> 12) & 0xFu);
        if (m_sel_excom_irl)
        {
            m_excom_out = excom_out;
        }
        if (m_sel_excom_irm)
        {
            m_excom_out = excom_out2;
        }
        if (m_sel_excom_irn)
        {
            m_excom_out = excom_out3;
        }
        if (m_sel_excom_irh)
        {
            m_excom_out = excom_out4;
        }
    }

    public bool GetAbsolutePath(string p1, string p2, out string path)
    {
        try
        {
            int length = p1.Length;
            if ((2 <= length && (p1[0] == Path.DirectorySeparatorChar || p1[0] == Path.AltDirectorySeparatorChar) && (p1[1] == Path.DirectorySeparatorChar || p1[1] == Path.AltDirectorySeparatorChar)) || (3 <= length && p1[1] == Path.VolumeSeparatorChar && (p1[2] == Path.DirectorySeparatorChar || p1[2] == Path.AltDirectorySeparatorChar)))
            {
                path = p1;
            }
            else
            {
                path = new Uri(new Uri(p2), p1).LocalPath;
            }
        }
        catch (Exception)
        {
            path = "";
            return false;
        }
        return true;
    }

    public int m_readIOfile()
    {
        string p = m_GetTargetName() + ".cs8";
        uint num = 0u;
        uint num2 = 0u;
        int result = 0;
        int num3 = 0;
        uint num4 = 0u;
        uint num5 = 0u;
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string pdrive = "";
        string pdir = "";
        string cpath = "";
        bool flag = baseDirectory.StartsWith("/", StringComparison.Ordinal);
        string path;
        if (flag)
        {
            LIBC.AdjustMacPath(baseDirectory, out cpath);
            GetAbsolutePath(p, string.Format("{0}{1}", cpath, "Resources/"), out path);
        }
        else
        {
            LIBC.SplitPath(baseDirectory, out pdrive, out pdir);
            GetAbsolutePath(p, baseDirectory, out path);
        }
        if (!File.Exists(path))
        {
            return 24;
        }
        uint num6 = 0u;
        ProfileStringReader profileStringReader = new ProfileStringReader(path);
        m_cs8_version = profileStringReader.GetPrivateProfileInt("FILE_INFO", "VERSION", 0u);
        string secname = "CORE";
        profileStringReader.GetPrivateProfileString(secname, "FAMILY_ID", "U8", out var result2);
        if (result2 == "U8")
        {
            m_cpu_mode = 0;
            m_inst_mode = 0;
        }
        else if (result2 == "U16")
        {
            m_cpu_mode = 1;
            m_inst_mode = 1;
        }
        else
        {
            m_cpu_mode = 0;
            m_inst_mode = 0;
            result = 25;
        }
        m_cpu_series = profileStringReader.GetPrivateProfileInt(secname, "SERIES", 0u);
        secname = "MEMORY";
        profileStringReader.GetPrivateProfileString(secname, "NAME", "", out result2);
        if (!string.IsNullOrEmpty(result2))
        {
            string text = ((!flag) ? $"{pdrive}\\{pdir}\\{result2}" : string.Format("{0}{1}{2}", cpath, "MonoBundle/", result2));
            if (text.Length >= 256)
            {
                num3 = -1;
                result = 26;
            }
            else
            {
                LIBC.strcpy_s(out DllMem_name, text);
                if (!File.Exists(DllMem_name))
                {
                    num3 = -1;
                    result = 27;
                }
                else
                {
                    DllMem_wait = BM.UI2I(profileStringReader.GetPrivateProfileInt(secname, "GM_WAITCYC", 0u));
                    if (DllMem_wait < 0 || 64 < DllMem_wait)
                    {
                        result = 28;
                        DllMem_wait = 0;
                    }
                }
            }
        }
        profileStringReader.GetPrivateProfileString("DEBUG", "NAME", "", out result2);
        if (!string.IsNullOrEmpty(result2))
        {
            string text = ((!flag) ? $"{pdrive}\\{pdir}\\{result2}" : string.Format("{0}{1}{2}", cpath, "MonoBundle/", result2));
            if (text.Length >= 256)
            {
                result = 29;
            }
            else
            {
                LIBC.strcpy_s(out DllDbg_name, text);
                if (!File.Exists(DllDbg_name))
                {
                    result = 30;
                }
            }
        }
        secname = "COPRO";
        profileStringReader.GetPrivateProfileString(secname, "NAME", "", out result2);
        if (!string.IsNullOrEmpty(result2))
        {
            string text = ((!flag) ? $"{pdrive}\\{pdir}\\{result2}" : string.Format("{0}{1}{2}", cpath, "MonoBundle/", result2));
            if (text.Length >= 256)
            {
                result = 31;
            }
            else
            {
                LIBC.strcpy_s(out DllCopro_name, text);
                if (!File.Exists(DllCopro_name))
                {
                    result = 32;
                }
                else
                {
                    DllCopro_id = profileStringReader.GetPrivateProfileInt(secname, "COPRO_ID", 0u);
                    if (DllCopro_id == 0 || 255 < DllCopro_id)
                    {
                        result = 33;
                    }
                }
            }
        }
        secname = "PERIPHERAL";
        num6 = profileStringReader.GetPrivateProfileInt(secname, "PERIPHERAL_NUM", 0u);
        if (num6 < 0 || 32 < num6)
        {
            result = 35;
            num6 = 0u;
        }
        else
        {
            DllNum_SimPeri = num6;
        }
        num = num6;
        for (uint num7 = 0u; num7 < num; num7++)
        {
            num3 = 0;
            profileStringReader.GetPrivateProfileString(secname, $"NAME{num7 + 1}", "", out result2);
            LIBC.strcpy_s(DllPeri_info[num7].dll_name, result2);
            if (string.IsNullOrEmpty(result2))
            {
                continue;
            }
            string text = ((!flag) ? $"{pdrive}\\{pdir}\\{result2}" : string.Format("{0}{1}{2}", cpath, "MonoBundle/", result2));
            if (text.Length >= 256)
            {
                result = 34;
                num3 = -1;
                continue;
            }
            LIBC.strcpy_s(out DllPeri_info[num7].dll_name_st, text);
            if (!File.Exists(text))
            {
                num3 = -1;
            }
            DllPeri_info[num7].dllstr = result2;
            if (num3 != 0)
            {
                continue;
            }
            num6 = getStringToUInt(profileStringReader, secname, $"PERIPHERAL{num7 + 1}_START", "0x00", 0u);
            if (num6 < 0 || 65535 < num6)
            {
                result = 36;
                num6 = 0u;
            }
            else
            {
                DllPeri_info[num7].start_adr = BM.UI2W(num6);
            }
            num6 = getStringToUInt(profileStringReader, secname, $"INT{num7 + 1}_NUM", "0x00", 0u);
            if (num6 < 0 || 128 < num6)
            {
                result = 37;
                num6 = 0u;
            }
            else
            {
                DllPeri_info[num7].int_num = BM.UI2W(num6);
            }
            num2 = num6;
            for (uint num8 = 0u; num8 < num2; num8++)
            {
                num6 = getStringToUInt(profileStringReader, secname, $"INT{num7 + 1}_VECTOR{num8 + 1}", "0x00", 0u);
                if (num6 < 0 || 65535 < num6)
                {
                    result = 38;
                    break;
                }
                DllPeri_info[num7].intInfo[num8].vec_adr = BM.UI2W(num6);
                num6 = getStringToUInt(profileStringReader, secname, $"INT{num7 + 1}_IEADR{num8 + 1}", "0x00", 0u);
                if (num6 < 0 || 65535 < num6)
                {
                    result = 39;
                    break;
                }
                DllPeri_info[num7].intInfo[num8].ie_adr = BM.UI2W(num6);
                num6 = profileStringReader.GetPrivateProfileInt(secname, $"INT{num7 + 1}_IEBIT{num8 + 1}", 0u);
                if (num6 < 0 || 7 < num6)
                {
                    result = 40;
                    break;
                }
                DllPeri_info[num7].intInfo[num8].ie_bit = BM.UI2W(num6);
                num6 = getStringToUInt(profileStringReader, secname, $"INT{num7 + 1}_IRQADR{num8 + 1}", "", 0u);
                if (num6 < 0 || 65535 < num6)
                {
                    result = 41;
                    break;
                }
                DllPeri_info[num7].intInfo[num8].irq_adr = BM.UI2W(num6);
                num6 = profileStringReader.GetPrivateProfileInt(secname, $"INT{num7 + 1}_IRQBIT{num8 + 1}", 0u);
                if (num6 < 0 || 7 < num6)
                {
                    result = 42;
                    break;
                }
                DllPeri_info[num7].intInfo[num8].irq_bit = BM.UI2W(num6);
                profileStringReader.GetPrivateProfileString(secname, $"INT{num7 + 1}_SYMBOL{num8 + 1}", "", out result2);
                LIBC.strcpy_s(DllPeri_info[num7].intInfo[num8].sym_name, result2);
                if (string.IsNullOrEmpty(BM.BA2S(DllPeri_info[num7].intInfo[num8].sym_name)))
                {
                    result = 43;
                    break;
                }
            }
        }
        secname = "STOP_HALT_INFO";
        num4 = getStringToUInt(profileStringReader, secname, "STPACP_ADDR", "0xF008", 61448u);
        num5 = getStringToUInt(profileStringReader, secname, "STPHLT_JDG_ADDR", "0xF009", 61449u);
        bool flag2 = true;
        if (m_cs8_version == 0)
        {
            if (num4 == 0 && num5 == 0)
            {
                flag2 = false;
            }
            else if (num4 == 0 && num5 != 0)
            {
                flag2 = true;
                num4 = 61464u;
            }
            else if (num4 != 0 && num5 == 0)
            {
                flag2 = false;
                result = 44;
            }
            else
            {
                flag2 = true;
            }
        }
        else if (num4 == 0 && num5 == 0)
        {
            flag2 = false;
        }
        else if (num4 == 0 || num5 == 0)
        {
            flag2 = false;
            result = 44;
        }
        else
        {
            flag2 = true;
        }
        if (flag2)
        {
            if (num4 <= 65535 && num5 <= 65535)
            {
                m_STPACP = num4;
                m_SBYCON = num5;
                int stringToInt = getStringToInt(profileStringReader, secname, "HLT_JDG_BITM", "0x0000", 0);
                int stringToInt2 = getStringToInt(profileStringReader, secname, "HLT_JDG_VALUE", "0x0001", 1);
                int stringToInt3 = getStringToInt(profileStringReader, secname, "STP_JDG_BITM", "0x0001", 1);
                int stringToInt4 = getStringToInt(profileStringReader, secname, "STP_JDG_VALUE", "0x0001", 1);
                int num9 = getStringToInt(profileStringReader, secname, "HLTH_JDG_BITM", "0x0002", 2);
                int num10 = getStringToInt(profileStringReader, secname, "HLTH_JDG_VALUE", "0x0001", 1);
                int num11 = getStringToInt(profileStringReader, secname, "STPD_JDG_BITM", "0x0003", 3);
                int num12 = getStringToInt(profileStringReader, secname, "STPD_JDG_VALUE", "0x0001", 1);
                int num13 = getStringToInt(profileStringReader, secname, "HLTC_JDG_BITM", "0x0004", 4);
                int num14 = getStringToInt(profileStringReader, secname, "HLTC_JDG_VALUE", "0x0001", 1);
                int num15 = getStringToInt(profileStringReader, secname, "STPSV1_JDG_BITM", "0x0004", 4);
                int num16 = getStringToInt(profileStringReader, secname, "STPSV1_JDG_VALUE", "0x0001", 1);
                int num17 = getStringToInt(profileStringReader, secname, "HLTSV1_JDG_BITM", "0x0004", 4);
                int num18 = getStringToInt(profileStringReader, secname, "HLTSV1_JDG_VALUE", "0x0001", 1);
                int num19 = getStringToInt(profileStringReader, secname, "STPSV2_JDG_BITM", "0x0004", 4);
                int num20 = getStringToInt(profileStringReader, secname, "STPSV2_JDG_VALUE", "0x0001", 1);
                if (num9 == 0)
                {
                    num9 = stringToInt;
                    num10 = stringToInt2;
                }
                if (num11 == 0)
                {
                    num11 = stringToInt3;
                    num12 = stringToInt4;
                }
                if (num13 == 0)
                {
                    num13 = stringToInt;
                    num14 = stringToInt2;
                }
                if (num15 == 0)
                {
                    num15 = stringToInt3;
                    num16 = stringToInt2;
                }
                if (num17 == 0)
                {
                    num17 = stringToInt;
                    num18 = stringToInt4;
                }
                if (num19 == 0)
                {
                    num19 = stringToInt3;
                    num20 = stringToInt4;
                }
                m_Halt_Jdg_bit_mask = (uint)(1 << stringToInt);
                m_Halt_Jdg_val = (uint)(stringToInt2 << stringToInt);
                m_Stop_Jdg_bit_mask = (uint)(1 << stringToInt3);
                m_Stop_Jdg_val = (uint)(stringToInt4 << stringToInt3);
                m_HaltH_Jdg_bit_mask = (uint)(1 << num9);
                m_HaltH_Jdg_val = (uint)(num10 << num9);
                m_StopD_Jdg_bit_mask = (uint)(1 << num11);
                m_StopD_Jdg_val = (uint)(num12 << num11);
                m_HaltC_Jdg_bit_mask = (uint)(1 << num13);
                m_HaltC_Jdg_val = (uint)(num14 << num13);
                m_StopSV1_Jdg_bit_mask = (uint)(1 << num15);
                m_StopSV1_Jdg_val = (uint)(num16 << num15);
                m_HaltSV1_Jdg_bit_mask = (uint)(1 << num17);
                m_HaltSV1_Jdg_val = (uint)(num18 << num17);
                m_StopSV2_Jdg_bit_mask = (uint)(1 << num19);
                m_StopSV2_Jdg_val = (uint)(num20 << num19);
            }
            else
            {
                result = 44;
            }
        }
        return result;
    }

    public uint getStringToUInt(ProfileStringReader prof_reader, string secname, string dname, string initSval, uint initIval)
    {
        uint num = initIval;
        prof_reader.GetPrivateProfileString(secname, dname, initSval, out var result);
        try
        {
            return Convert.ToUInt32(result, 16);
        }
        catch
        {
            return initIval;
        }
    }

    public int getStringToInt(ProfileStringReader prof_reader, string secname, string dname, string initSval, int initIval)
    {
        int num = initIval;
        prof_reader.GetPrivateProfileString(secname, dname, initSval, out var result);
        try
        {
            return Convert.ToInt32(result, 16);
        }
        catch
        {
            return initIval;
        }
    }

    public void m_initPeriData()
    {
        m_trg_name = "_default";
        m_cpu_mode = 0;
        DllCopro_id = 0u;
        DllNum_SimPeri = 0u;
        DllMem_wait = 0;
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 260; j++)
            {
                DllPeri_info[i].InitIntInfo();
            }
            DllPeri_info[i].start_adr = 0;
            DllPeri_info[i].int_num = 0;
            for (int k = 0; k < 128; k++)
            {
                DllPeri_info[i].intInfo[k].vec_adr = 0;
                DllPeri_info[i].intInfo[k].ie_adr = 0;
                DllPeri_info[i].intInfo[k].ie_bit = 0;
                DllPeri_info[i].intInfo[k].irq_adr = 0;
                DllPeri_info[i].intInfo[k].irq_bit = 0;
                for (int l = 0; l < 32; l++)
                {
                    DllPeri_info[i].intInfo[k].InitSymName();
                    DllPeri_info[i].intInfo[k].sym_name[l] = 0;
                }
            }
        }
    }

    public void m_DynamicLink()
    {
        //if (!string.IsNullOrEmpty(DllCopro_name))
        //{
        CSimCoproApp = new SimCopro.CSimCoproApp(); //Activator.CreateInstance(Assembly.LoadFrom(DllCopro_name).GetType("SimCopro.CSimCoproApp"));
        //if (CSimCoproApp != null)
        //{
        //    byte b = (byte)(DllCopro_id & 0xFFu);
        //    _ = (int)CSimCoproApp.cpApi_SetCoproID(b);
        //}
        // }
        CSimDbgApp = new SimDbg.CSimDbgApp();// Activator.CreateInstance(Assembly.LoadFrom(DllDbg_name).GetType("SimDbg.CSimDbgApp"));
        CSimMemApp = new SimMem.CSimMemApp();// Activator.CreateInstance(Assembly.LoadFrom(DllMem_name).GetType("SimMem.CSimMemApp"));
        hSimPeripheralDLLInst = new object[32];
        for (int i = 0; i < DllNum_SimPeri; i++)
        {
            tPeriIntInfo[] array = new tPeriIntInfo[128];
            for (int j = 0; j != 128; j++)
            {
                array[j].InitIntSym();
            }
            dynamic val = Activator.CreateInstance(Assembly.LoadFrom(DllPeri_info[i].dllstr).GetModule(DllPeri_info[i].dllstr).GetType($"{Path.GetFileNameWithoutExtension(DllPeri_info[i].dllstr)}.CSimPeripheralApp"));
            hSimPeripheralDLLInst[i] = val;
            val.perApi_SetSFRStartAdr(DllPeri_info[i].start_adr);
            val.perApi_SetMemDllName(DllMem_name);
            for (int k = 0; k < DllPeri_info[i].int_num; k++)
            {
                array[k].Irq_Addr = DllPeri_info[i].intInfo[k].irq_adr;
                array[k].Irq_Bit = DllPeri_info[i].intInfo[k].irq_bit;
                array[k].InitIntSym();
                LIBC.strcpy_s(array[k].IntSym, DllPeri_info[i].intInfo[k].sym_name);
            }
            val.perApi_SetIntInfo(DllPeri_info[i].int_num, array);
            BRKPARAM bRKPARAM = default(BRKPARAM);
            bRKPARAM.adrbrk_adrs = 1u;
            bRKPARAM.adrbrk_pcnt = 2;
            bRKPARAM.InitDMParam();
            bRKPARAM.dm_param[0].access = 10;
            bRKPARAM.dm_pcnt = 3;
            bRKPARAM.dm_pair = 4;
            bRKPARAM.brkcond = 5;
            CSimDbgApp.dbgApi_GetBreakParam(ref bRKPARAM);
        }
    }

    public void exe_rn_imm8(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 12) & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_dend_id = true;
            m_sel_excom_irh = true;
            m_greg0_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_sel_cbus1_irl_id = true;
            m_exe_rn_rm_id = true;
            m_dtword_id = false;
            switch (b)
            {
                case 1:
                    m_alu_add_flag = true;
                    break;
                case 6:
                    m_alu_adc_flag = true;
                    break;
                case 2:
                    m_alu_and_flag = true;
                    break;
                case 7:
                    m_alu_cmp_flag = true;
                    break;
                case 5:
                    m_alu_cpc_flag = true;
                    break;
                case 0:
                    m_alu_mov_flag = true;
                    break;
                case 3:
                    m_alu_or_flag = true;
                    break;
                case 4:
                    m_alu_xor_flag = true;
                    break;
            }
        }
    }

    public void exe_rn_rm(ushort irdata, int state)
    {
        byte b = (byte)(irdata & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_dend_id = true;
            m_exe_rn_rm_id = true;
            m_sel_excom_irl = true;
            m_greg0_entry_id = true;
            m_greg1_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_dtword_id = false;
            switch (b)
            {
                case 1:
                    m_alu_add_flag = true;
                    break;
                case 6:
                    m_alu_adc_flag = true;
                    break;
                case 2:
                    m_alu_and_flag = true;
                    break;
                case 7:
                    m_alu_cmp_flag = true;
                    break;
                case 5:
                    m_alu_cpc_flag = true;
                    break;
                case 0:
                    m_alu_mov_flag = true;
                    break;
                case 3:
                    m_alu_or_flag = true;
                    break;
                case 4:
                    m_alu_xor_flag = true;
                    break;
                case 8:
                    m_alu_sub_flag = true;
                    break;
                case 9:
                    m_alu_sbc_flag = true;
                    break;
            }
        }
    }

    public void shift_rn_rm(ushort irdata, int state)
    {
        byte b = (byte)(irdata & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_dend_id = true;
            m_greg0_entry_id = true;
            m_greg1_entry_id = true;
            m_greg2_entry_id = true;
            m_sel_excom_irl = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_shift_id = true;
            m_sel_abus_swap_id = true;
            m_sel_a16r_abus_id = true;
            m_wr_arl_id = true;
            m_wr_arh_id = true;
            m_dtword_id = false;
            m_wr_greg_wb_id = true;
            switch (b)
            {
                case 10:
                    m_left_id = true;
                    m_shift_sll_flag = true;
                    break;
                case 11:
                    m_left_id = true;
                    m_shift_sllc_flag = true;
                    break;
                case 14:
                    m_right_id = true;
                    m_shift_sra_flag = true;
                    break;
                case 12:
                    m_right_id = true;
                    m_shift_srl_flag = true;
                    break;
                case 13:
                    m_right_id = true;
                    m_shift_srlc_flag = true;
                    break;
            }
        }
    }

    public void task_extend(ushort irdata, int state)
    {
        m_dstart_id = true;
        m_dend_id = true;
        m_greg0_entry_id = true;
        m_greg1_entry_id = true;
        m_sel_ex2_cbus1_id = true;
        m_exe_alu_id = true;
        m_sel_excom_irm = true;
        m_shift_signextend_flag = true;
        m_dtword_id = false;
        m_wr_greg_wb_id = true;
        m_alu_reverse_flag = true;
    }

    public void task_daa(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_dend_id = true;
            m_greg0_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_cbus1_daa_id = true;
            m_sel_ex2_cbus1_id = true;
            m_sel_excom_irm = true;
            m_exe_alu_id = true;
            m_wr_greg_wb_id = true;
            m_dtword_id = false;
            switch (b)
            {
                case 1:
                    m_alu_daa_flag = true;
                    break;
                case 3:
                    m_alu_das_flag = true;
                    break;
            }
        }
    }

    public void task_neg(ushort irdata, int state)
    {
        m_dstart_id = true;
        m_dend_id = true;
        m_greg0_entry_id = true;
        m_neg_id = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_ex2_cbus1_id = true;
        m_sel_excom_irm = true;
        m_wr_greg_wb_id = true;
        m_alu_sub_flag = true;
        m_dtword_id = false;
    }

    public void task_lod_st(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF0FFu);
            if (num2 == 36916 || num2 == 36948 || num2 == 36918 || num2 == 36950 || num2 == 36917 || num2 == 36949 || num2 == 36919 || num2 == 36951)
            {
                num = 1;
                switch (state)
                {
                    case 0:
                        m_dstart_id = true;
                        m_ea_mode_id = true;
                        m_greg0_entry_id = true;
                        m_sel_adbus_ea_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_pc_wait_id = true;
                        m_sel_abus_bound_id = true;
                        m_dtword_id = true;
                        if (num2 == 36916 || num2 == 36948 || num2 == 36918 || num2 == 36950)
                        {
                            m_memory_load_id = true;
                            m_exe_wr_z_id = true;
                        }
                        else
                        {
                            m_memory_store_id = true;
                            m_sel_ex1_cbus0_id = true;
                        }
                        break;
                    case 1:
                        m_ea_mode_id = true;
                        m_greg0_entry_id = true;
                        m_sel_greg0_regn_bit1or_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_dtword_id = true;
                        if (num2 == 36916 || num2 == 36948 || num2 == 36918 || num2 == 36950)
                        {
                            if (num2 == 36916 || num2 == 36948)
                            {
                                m_dend_id = true;
                                if (num2 == 36948)
                                {
                                    m_ea_plus_id = true;
                                }
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                            m_memory_load_id = true;
                            m_exe_wr_zand_id = true;
                            break;
                        }
                        if (num2 == 36917 || num2 == 36949)
                        {
                            m_dend_id = true;
                            if (num2 == 36949)
                            {
                                m_ea_plus_id = true;
                            }
                        }
                        else
                        {
                            m_pc_wait_id = true;
                        }
                        m_sel_ex1_cbus0_id = true;
                        m_memory_store_id = true;
                        break;
                    case 2:
                        if (num2 == 36918 || num2 == 36950 || num2 == 36919 || num2 == 36951)
                        {
                            m_ea_mode_id = true;
                            m_greg0_entry_id = true;
                            m_sel_greg0_regn_bit2or_id = true;
                            m_sel_a16l_eabus_id = true;
                            m_sel_a16r_1_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            m_pc_wait_id = true;
                            m_dtword_id = true;
                            if (num2 == 36918 || num2 == 36950)
                            {
                                m_memory_load_id = true;
                                m_exe_wr_zand_id = true;
                            }
                            else
                            {
                                m_sel_ex1_cbus0_id = true;
                                m_memory_store_id = true;
                            }
                        }
                        break;
                    case 3:
                        if (num2 != 36918 && num2 != 36950 && num2 != 36919 && num2 != 36951)
                        {
                            break;
                        }
                        m_ea_mode_id = true;
                        m_greg0_entry_id = true;
                        m_sel_greg0_regn_bit1or_id = true;
                        m_sel_greg0_regn_bit2or_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_dend_id = true;
                        m_dtword_id = true;
                        if (num2 == 36918 || num2 == 36950)
                        {
                            if (num2 == 36950)
                            {
                                m_ea_plus_id = true;
                            }
                            m_memory_load_id = true;
                            m_exe_wr_zand_id = true;
                        }
                        else
                        {
                            if (num2 == 36951)
                            {
                                m_ea_plus_id = true;
                            }
                            m_sel_ex1_cbus0_id = true;
                            m_memory_store_id = true;
                        }
                        break;
                }
            }
        }
        if (num != 0)
        {
            return;
        }
        byte b = (byte)((uint)(irdata >> 4) & 7u);
        switch (b)
        {
            case 1:
                task_lst_reg_d16(irdata, state);
                return;
            case 7:
                min_cycle(irdata, state);
                return;
        }
        if ((b & 1) == 0)
        {
            task_lst_reg_erm(irdata, state);
        }
        else
        {
            task_lst_reg_ea(irdata, state);
        }
    }

    public void task_lst_reg_d16(ushort irdata, int state)
    {
        m_IdentfyInstLST(irdata);
        if (m_inst_mode != 0 && !m_byte_mode)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_sel_cbus1_roml_id = true;
                    m_sel_cbus2_romh_id = true;
                    m_sel_a16r_abus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_dtword_id = true;
                    break;
                case 1:
                    if (m_core_rev_type != 0 && m_core_rev_type != 1)
                    {
                        m_sel_abus_bound_id = true;
                    }
                    m_greg0_entry_id = true;
                    m_greg1_entry_id = true;
                    m_dend_id = true;
                    if (m_load_mode)
                    {
                        m_memory_load_id = true;
                        m_exe_wr_z_id = true;
                    }
                    else
                    {
                        m_sel_ex1_cbus0_id = true;
                        m_memory_store_id = true;
                    }
                    m_dtword_id = true;
                    break;
            }
            return;
        }
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_greg0_entry_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if (!m_byte_mode && (m_inst_mode == 0 || (m_core_rev_type != 0 && m_core_rev_type != 1)))
                {
                    m_sel_abus_bound_id = true;
                }
                if (m_load_mode)
                {
                    if (!m_byte_mode)
                    {
                        m_memory_load_id = true;
                        m_exe_wr_z_id = true;
                    }
                }
                else
                {
                    m_sel_ex1_cbus0_id = true;
                    if (!m_byte_mode)
                    {
                        m_memory_store_id = true;
                    }
                }
                m_dtword_id = false;
                break;
            case 1:
                m_dend_id = true;
                if (!m_byte_mode)
                {
                    m_sel_greg0_regn_bit0or_id = true;
                    m_greg0_entry_id = true;
                    task_inc_eabus();
                    if (m_load_mode)
                    {
                        m_memory_load_id = true;
                        m_exe_wr_zand_id = true;
                    }
                    else
                    {
                        m_sel_ex1_cbus0_id = true;
                        m_memory_store_id = true;
                    }
                }
                else
                {
                    m_greg0_entry_id = true;
                    if (m_load_mode)
                    {
                        m_memory_load_id = true;
                        m_exe_wr_z_id = true;
                    }
                    else
                    {
                        m_sel_ex1_cbus0_id = true;
                        m_memory_store_id = true;
                    }
                }
                m_dtword_id = false;
                break;
        }
    }

    public void task_lst_reg_erm(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF00Fu);
            if (num2 == 36866 || num2 == 36867)
            {
                num = 1;
                if (state == 0)
                {
                    m_dstart_id = true;
                    m_sel_a16r_abus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    if (m_core_rev_type != 0 && m_core_rev_type != 1)
                    {
                        m_sel_abus_bound_id = true;
                    }
                    m_greg0_entry_id = true;
                    m_greg1_entry_id = true;
                    m_greg2_entry_id = true;
                    m_dend_id = true;
                    m_dtword_id = true;
                    if (num2 == 36866)
                    {
                        m_memory_load_id = true;
                        m_exe_wr_z_id = true;
                    }
                    else
                    {
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                    }
                }
            }
        }
        if (num != 0)
        {
            return;
        }
        m_IdentfyInstLST(irdata);
        if (state == 0 || state == 1)
        {
            if (m_load_mode)
            {
                m_memory_load_id = true;
            }
            else
            {
                m_sel_ex1_cbus0_id = true;
                m_memory_store_id = true;
            }
        }
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if (m_byte_mode)
                {
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                    m_sel_abus_bound_id = true;
                }
                m_greg0_entry_id = true;
                m_greg1_entry_id = true;
                m_greg2_entry_id = true;
                if (m_load_mode)
                {
                    m_exe_wr_z_id = true;
                }
                m_dtword_id = false;
                break;
            case 1:
                task_inc_eabus();
                m_sel_greg0_regn_bit0or_id = true;
                m_greg0_entry_id = true;
                m_dend_id = true;
                if (m_load_mode)
                {
                    m_exe_wr_zand_id = true;
                }
                m_dtword_id = false;
                break;
        }
    }

    public void task_lst_reg_ea(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF0FFu);
            if (num2 == 36914 || num2 == 36946 || num2 == 36915 || num2 == 36947)
            {
                num = 1;
                if (state == 0)
                {
                    m_ea_mode_id = true;
                    m_greg0_entry_id = true;
                    m_dstart_id = true;
                    m_sel_adbus_ea_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_sel_abus_bound_id = true;
                    m_dend_id = true;
                    m_dtword_id = true;
                    if (num2 == 36914 || num2 == 36946)
                    {
                        if (num2 == 36946)
                        {
                            m_ea_plus_id = true;
                        }
                        m_memory_load_id = true;
                        m_exe_wr_z_id = true;
                    }
                    else
                    {
                        if (num2 == 36947)
                        {
                            m_ea_plus_id = true;
                        }
                        m_sel_ex1_cbus0_id = true;
                        m_memory_store_id = true;
                    }
                }
            }
        }
        if (num != 0)
        {
            return;
        }
        m_IdentfyInstLST(irdata);
        m_ea_mode_id = true;
        m_greg0_entry_id = true;
        task_register_inc(state);
        if (m_load_mode)
        {
            m_memory_load_id = true;
        }
        else
        {
            m_sel_ex1_cbus0_id = true;
            m_memory_store_id = true;
        }
        if (state == 0)
        {
            m_sel_adbus_ea_id = true;
            m_sel_arbus_eabus_id = true;
            m_wr_arl_id = true;
            m_wr_arh_id = true;
            if (m_byte_mode)
            {
                m_dend_id = true;
                if (m_eaplus_mode)
                {
                    m_ea_plus_id = true;
                }
            }
            else
            {
                m_pc_wait_id = true;
                m_sel_abus_bound_id = true;
            }
            if (m_load_mode)
            {
                m_exe_wr_z_id = true;
            }
            m_dtword_id = false;
            return;
        }
        m_sel_a16l_eabus_id = true;
        m_sel_a16r_1_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
        m_dtword_id = false;
        if (m_load_mode)
        {
            m_exe_wr_zand_id = true;
        }
        switch (state)
        {
            case 1:
                if (m_word_mode)
                {
                    m_dend_id = true;
                    if (m_eaplus_mode)
                    {
                        m_ea_plus_id = true;
                    }
                }
                else
                {
                    m_pc_wait_id = true;
                }
                break;
            case 3:
                if (m_dword_mode)
                {
                    m_dend_id = true;
                    if (m_eaplus_mode)
                    {
                        m_ea_plus_id = true;
                    }
                }
                else
                {
                    m_pc_wait_id = true;
                }
                break;
            case 7:
                m_dend_id = true;
                if (m_eaplus_mode)
                {
                    m_ea_plus_id = true;
                }
                break;
            default:
                m_pc_wait_id = true;
                break;
        }
    }

    public void lst_rn_d16ern(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF00Fu);
            if ((uint)(num2 - 40968) <= 1u)
            {
                num = 1;
                switch (state)
                {
                    case 0:
                        m_dstart_id = true;
                        m_sel_cbus1_roml_id = true;
                        m_sel_cbus2_romh_id = true;
                        m_sel_a16r_abus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_dtword_id = true;
                        break;
                    case 1:
                        if (m_core_rev_type != 0 && m_core_rev_type != 1)
                        {
                            m_sel_abus_bound_id = true;
                        }
                        m_sel_excom_irl = true;
                        m_greg0_entry_id = true;
                        m_greg1_entry_id = true;
                        m_greg2_entry_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_abus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_dend_id = true;
                        m_dtword_id = true;
                        if (num2 == 40968)
                        {
                            m_memory_load_id = true;
                            m_exe_wr_z_id = true;
                        }
                        else
                        {
                            m_memory_store_id = true;
                            m_sel_ex1_cbus0_id = true;
                        }
                        break;
                }
            }
        }
        if (num != 0)
        {
            return;
        }
        byte b = (byte)((uint)(irdata >> 12) & 0xFu);
        byte b2 = (byte)(irdata & 0xFu);
        m_IdentfyInstLST(irdata);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dtword_id = false;
                break;
            case 1:
                if ((b & 1) == 0)
                {
                    m_pc_wait_id = true;
                    m_sel_abus_bound_id = true;
                }
                else
                {
                    m_dend_id = true;
                }
                m_sel_excom_irl = true;
                m_greg0_entry_id = true;
                m_greg1_entry_id = true;
                m_greg2_entry_id = true;
                task_add_ar_abus();
                if (((uint)b2 & (true ? 1u : 0u)) != 0)
                {
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                }
                else
                {
                    m_memory_load_id = true;
                    m_exe_wr_z_id = true;
                }
                m_dtword_id = false;
                break;
            case 2:
                m_dend_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                m_greg0_entry_id = true;
                m_task_inc_eabus();
                m_sel_excom_irl = true;
                if (((uint)b2 & (true ? 1u : 0u)) != 0)
                {
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                }
                else
                {
                    m_memory_load_id = true;
                    m_exe_wr_zand_id = true;
                }
                m_dtword_id = false;
                break;
        }
    }

    public void shift_rn_imm8(ushort irdata, int state)
    {
        byte b = (byte)(irdata & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_dend_id = true;
            m_sel_excom_irl = true;
            m_sel_cbus2_irl_id = true;
            m_greg0_entry_id = true;
            m_greg1_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_shift_id = true;
            m_sel_abus_width_id = true;
            m_sel_a16r_abus_id = true;
            m_wr_arl_id = true;
            m_wr_arh_id = true;
            m_dtword_id = false;
            m_wr_greg_wb_id = true;
            switch (b)
            {
                case 10:
                    m_shift_sll_flag = true;
                    m_left_id = true;
                    break;
                case 11:
                    m_shift_sllc_flag = true;
                    m_left_id = true;
                    break;
                case 14:
                    m_right_id = true;
                    m_shift_sra_flag = true;
                    break;
                case 12:
                    m_right_id = true;
                    m_shift_srl_flag = true;
                    break;
                case 13:
                    m_right_id = true;
                    m_shift_srlc_flag = true;
                    break;
            }
        }
    }

    public void mov_dsr_rn(ushort irdata, int state)
    {
        m_dstart_id = true;
        m_dend_id = true;
        m_greg1_entry_id = true;
        m_sel_ex2_cbus1_id = true;
        m_cntl_move_id = true;
        m_sel_excom_irn = true;
        m_edsr_id = true;
        m_disint_id = true;
        m_state_clr_id = true;
        m_alu_mov_flag = true;
    }

    public void task_sb_rb_tb(ushort irdata, int state)
    {
        byte b = (byte)(irdata & 0xFu);
        min_cycle(irdata, state);
        m_sel_excom_irl = true;
        m_exebit_id = true;
        m_sel_cbus1_irl_id = true;
        m_greg0_entry_id = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_ex2_cbus1_id = true;
        m_dtword_id = false;
        switch (b)
        {
            case 0:
                m_alu_sb_flag = true;
                m_wr_greg_wb_id = true;
                break;
            case 2:
                m_alu_rb_flag = true;
                m_wr_greg_wb_id = true;
                break;
            case 1:
                m_alu_rb_flag = true;
                break;
        }
    }

    public void task_mov_rn_cntl(ushort irdata, int state)
    {
        byte b = (byte)((byte)(irdata & 0xFu) & 7u);
        m_sel_ex2_cbus1_id = true;
        m_data_move_id = true;
        m_greg0_entry_id = true;
        switch (b)
        {
            case 3:
                m_sel_cbus1_psw_id = true;
                min_cycle(irdata, state);
                m_dtword_id = false;
                m_alu_mov_flag = true;
                return;
            case 4:
                if (m_inst_mode == 0)
                {
                    switch (state)
                    {
                        case 0:
                            m_dstart_id = true;
                            m_pc_wait_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            break;
                        case 1:
                            m_sel_cbus1_epsw_id = true;
                            m_sel_ex2_cbus1_id = true;
                            m_greg0_entry_id = true;
                            m_dend_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            break;
                    }
                }
                else if (state == 0)
                {
                    m_sel_cbus1_epsw_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_greg0_entry_id = true;
                    m_dstart_id = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                    m_alu_mov_flag = true;
                }
                return;
            case 5:
                if (m_inst_mode == 0)
                {
                    switch (state)
                    {
                        case 0:
                            m_dstart_id = true;
                            m_pc_wait_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            break;
                        case 1:
                            m_sel_cbus1_elrl_id = true;
                            m_pc_wait_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            break;
                        case 2:
                            m_sel_greg0_regn_bit0or_id = true;
                            m_sel_cbus1_elrh_id = true;
                            m_dend_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            break;
                    }
                }
                else if (state == 0)
                {
                    m_data_move_id = true;
                    m_sel_cbus1_elrl_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_greg0_entry_id = true;
                    m_dstart_id = true;
                    m_dend_id = true;
                    m_dtword_id = true;
                    m_alu_mov_flag = true;
                }
                return;
        }
        if (m_inst_mode == 0)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    m_alu_mov_flag = true;
                    break;
                case 1:
                    m_sel_cbus1_ecsr_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_greg0_entry_id = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                    m_alu_mov_flag = true;
                    break;
            }
        }
        else if (state == 0)
        {
            m_sel_cbus1_ecsr_id = true;
            m_sel_ex2_cbus1_id = true;
            m_greg0_entry_id = true;
            m_dend_id = true;
            m_dstart_id = true;
            m_dtword_id = false;
            m_alu_mov_flag = true;
        }
    }

    public void mov_rn_crm(ushort irdata, int state)
    {
        min_cycle(irdata, state);
        m_greg0_entry_id = true;
        m_sel_excom_irm = true;
        m_cop_read_id = true;
        m_sel_r0_irl = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_ex2_cbus1_id = true;
        m_alu_mov_flag = true;
        m_dtword_id = false;
        m_wr_greg_wb_id = true;
    }

    public void mov_fp_sp(ushort irdata, int state)
    {
        if (m_inst_mode == 0)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_greg0_entry_id = true;
                    m_sel_cbus1_spl_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_data_move_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    m_alu_mov_flag = true;
                    break;
                case 1:
                    m_dend_id = true;
                    m_greg0_entry_id = true;
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_cbus1_sph_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_data_move_id = true;
                    m_dtword_id = false;
                    m_alu_mov_flag = true;
                    break;
            }
        }
        else if (state == 0)
        {
            m_dstart_id = true;
            m_greg0_entry_id = true;
            m_sel_cbus1_spl_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_data_move_id = true;
            m_dtword_id = true;
            m_alu_mov_flag = true;
            m_dend_id = true;
        }
    }

    public void mov_sp_fp(ushort irdata, int state)
    {
        min_cycle(irdata, state);
        m_greg1_entry_id = true;
        m_greg2_entry_id = true;
        m_sel_a16r_abus_id = true;
        m_wr_sp_id = true;
        m_dtword_id = false;
        m_alu_mov_flag = true;
    }

    public void task_mov_cntl_rm(ushort irdata, int state)
    {
        byte b = (byte)((byte)(irdata & 0xFu) & 7u);
        switch (b)
        {
            case 5:
                m_greg0_entry_id = true;
                m_sel_ex1_cbus0_id = true;
                switch (state)
                {
                    case 0:
                        m_greg1_entry_id = true;
                        m_dstart_id = true;
                        m_wr_elrl_wb_id = true;
                        m_alu_mov_flag = true;
                        m_alu_reverse_flag = true;
                        if (m_inst_mode == 0)
                        {
                            m_dtword_id = false;
                            m_pc_wait_id = true;
                        }
                        else
                        {
                            m_dend_id = true;
                            m_dtword_id = true;
                        }
                        break;
                    case 1:
                        if (m_inst_mode == 0)
                        {
                            m_greg1_entry_id = true;
                            m_sel_greg0_regn_bit0or_id = true;
                            m_wr_elrl_wb_id = true;
                            m_pc_wait_id = true;
                            m_dtword_id = false;
                            m_alu_mov_flag = true;
                            m_alu_reverse_flag = true;
                        }
                        break;
                    case 2:
                        if (m_inst_mode == 0)
                        {
                            m_dend_id = true;
                            m_dtword_id = false;
                        }
                        break;
                }
                break;
            case 7:
                switch (state)
                {
                    case 0:
                        m_cntl_axxx_id = true;
                        m_sel_excom_irl = true;
                        m_greg1_entry_id = true;
                        m_sel_ex2_cbus1_id = true;
                        m_dstart_id = true;
                        m_alu_mov_flag = true;
                        if (m_inst_mode == 0)
                        {
                            m_dtword_id = false;
                            m_pc_wait_id = true;
                        }
                        else
                        {
                            m_dend_id = true;
                            m_dtword_id = true;
                        }
                        break;
                    case 1:
                        if (m_inst_mode == 0)
                        {
                            m_dend_id = true;
                            m_dtword_id = false;
                        }
                        break;
                }
                break;
            default:
                m_cntl_axxx_id = true;
                m_sel_excom_irl = true;
                m_greg1_entry_id = true;
                m_sel_ex2_cbus1_id = true;
                min_cycle(irdata, state);
                if (b == 3)
                {
                    m_wr_psw_wb_id = true;
                }
                m_alu_mov_flag = true;
                break;
        }
    }

    public void mov_crn_rm(ushort irdata, int state)
    {
        min_cycle(irdata, state);
        m_greg0_entry_id = true;
        m_greg1_entry_id = true;
        m_sel_ex2_cbus1_id = true;
        m_cop_store_id = true;
        m_cntl_axxx_id = true;
        m_sel_excom_irl = true;
        m_alu_mov_flag = true;
        m_dtword_id = false;
        m_wr_greg_wb_id = true;
    }

    public void task_sb_rb_d16(ushort irdata, int state)
    {
        byte b = (byte)(irdata & 0xFu);
        switch (state)
        {
            case 0:
                if ((b & 1) == 0)
                {
                    m_dtlock_id = true;
                }
                m_dstart_id = true;
                m_memory_read_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_sel_excom_irl = true;
                m_dtword_id = false;
                break;
            case 1:
                m_sel_cbus1_irl_id = true;
                m_sel_ex2_cbus1_id = true;
                if ((b & 1) == 0)
                {
                    m_memory_store_id = true;
                }
                if ((b & 3) == 0)
                {
                    m_alu_sb_flag = true;
                }
                else
                {
                    m_alu_rb_flag = true;
                }
                m_dend_id = true;
                m_dtword_id = false;
                m_wr_ex1_wb_id = true;
                break;
        }
    }

    public void task_st_rn_disp6(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF0C0u);
            switch (num2)
            {
                case 45056:
                case 45120:
                case 45184:
                case 45248:
                    num = 1;
                    switch (state)
                    {
                        case 0:
                            m_dstart_id = true;
                            m_greg0_entry_id = true;
                            m_pc_wait_id = true;
                            m_sel_a16r_abus_id = true;
                            m_wr_disp6_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            m_dtword_id = true;
                            if (num2 == 45184 || num2 == 45248)
                            {
                                m_sel_ex1_cbus0_id = true;
                            }
                            break;
                        case 1:
                            if (num2 == 45056 || num2 == 45184)
                            {
                                m_sel_greg0_bp_id = true;
                            }
                            else
                            {
                                m_sel_greg0_fp_id = true;
                            }
                            m_greg0_entry_id = true;
                            m_sel_a16r_abus_id = true;
                            m_sel_a16l_eabus_id = true;
                            if (num2 == 45056 || num2 == 45120)
                            {
                                m_memory_load_id = true;
                                m_exe_wr_z_id = true;
                            }
                            else
                            {
                                m_memory_store_id = true;
                                m_sel_ex1_cbus0_id = true;
                            }
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            if (m_core_rev_type != 0 && m_core_rev_type != 1)
                            {
                                m_sel_abus_bound_id = true;
                            }
                            m_dend_id = true;
                            m_dtword_id = true;
                            break;
                    }
                    break;
            }
        }
        if (num != 0)
        {
            return;
        }
        byte b = (byte)((uint)(irdata >> 12) & 0xFu);
        byte b2 = (byte)((uint)(irdata >> 4) & 0xFu);
        m_IdentfyInstLST(irdata);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_greg0_entry_id = true;
                m_pc_wait_id = true;
                m_sel_a16r_abus_id = true;
                if ((b2 & 8) == 8)
                {
                    m_sel_ex1_cbus0_id = true;
                }
                m_wr_disp6_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dtword_id = false;
                break;
            case 1:
                if ((b2 & 4) == 4)
                {
                    m_sel_greg0_fp_id = true;
                }
                else
                {
                    m_sel_greg0_bp_id = true;
                }
                m_greg0_entry_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_a16l_eabus_id = true;
                if ((b2 & 8) == 8)
                {
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                }
                else
                {
                    m_memory_load_id = true;
                    m_exe_wr_z_id = true;
                }
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if ((b & 4) == 4)
                {
                    m_dend_id = true;
                }
                else
                {
                    m_sel_abus_bound_id = true;
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                break;
            case 2:
                m_greg0_entry_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                if ((b2 & 8) == 8)
                {
                    m_sel_ex1_cbus0_id = true;
                    m_memory_store_id = true;
                }
                else
                {
                    m_memory_load_id = true;
                    m_exe_wr_zand_id = true;
                }
                m_sel_a16l_eabus_id = true;
                m_sel_a16r_1_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_bcc_radr(ushort irdata, int state)
    {
        if (m_core_rev_type >= 3)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_bcc_id = true;
                    m_sel_excom_irn = true;
                    m_sel_cbus1_bcc_id = true;
                    m_sel_cbus2_irl_id = true;
                    m_sel_abus_swap_id = true;
                    m_sel_a16l_pc_id = true;
                    m_sel_a16r_abus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_dtword_id = false;
                    m_sel_a16l_eabus_id = true;
                    m_sel_pc_pcbus_id = true;
                    break;
                case 1:
                    m_dend_id = true;
                    m_dtword_id = false;
                    break;
            }
            return;
        }
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_bcc_id = true;
                m_sel_excom_irn = true;
                m_sel_cbus1_bcc_id = true;
                m_sel_cbus2_irl_id = true;
                m_sel_abus_swap_id = true;
                m_sel_a16l_pc_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dtword_id = false;
                m_sel_a16l_eabus_id = true;
                m_sel_pc_pcbus_id = true;
                break;
            case 1:
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 2:
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_exe_ern_imm7(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 7) & 1u);
        if ((uint)b > 1u)
        {
            return;
        }
        if (m_inst_mode == 0)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_cbus1_irl_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_sel_excom_irm = true;
                    m_exe_word3_id = true;
                    m_pc_wait_id = true;
                    m_shift_imm7_flag = true;
                    m_dtword_id = false;
                    if (b == 1)
                    {
                        m_alu_add_flag = true;
                    }
                    else
                    {
                        m_alu_mov_flag = true;
                    }
                    break;
                case 1:
                    m_greg0_entry_id = true;
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_exe_word3_id = true;
                    m_sel_excom_irm = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                    if (b == 1)
                    {
                        m_sel_cbus1_borrow_id = true;
                        m_alu_add_flag = true;
                    }
                    else
                    {
                        m_shift_imm7_flag = true;
                        m_sel_cbus1_irl_id = true;
                        m_alu_mov_flag = true;
                    }
                    break;
            }
        }
        else if (state == 0)
        {
            m_dstart_id = true;
            m_greg0_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_cbus1_irl_id = true;
            m_sel_ex2_cbus1_id = true;
            m_sel_excom_irm = true;
            m_exe_word3_id = true;
            m_dend_id = true;
            m_dtword_id = true;
            m_shift_imm7_flag = true;
            if (b == 1)
            {
                m_alu_add_flag = true;
            }
            else
            {
                m_alu_mov_flag = true;
            }
        }
    }

    public void add_sp_imm8(ushort irdata, int state)
    {
        if (m_inst_mode == 0)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_sel_cbus1_irl_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_extend_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    m_shift_extend_flag = true;
                    break;
                case 1:
                    m_dend_id = true;
                    m_sel_cbus1_irl_id = true;
                    m_sel_cbus2_wbdata_id = true;
                    m_sel_a16r_abus_id = true;
                    task_set_sp_ar();
                    m_dtword_id = false;
                    m_alu_adsp_flag = true;
                    break;
            }
        }
        else if (state == 0)
        {
            m_dstart_id = true;
            m_sel_cbus1_irl_id = true;
            m_extend_id = true;
            m_sel_a16r_abus_id = true;
            m_sel_eabus_sp_id = true;
            m_sel_a16l_eabus_id = true;
            m_wr_sp_id = true;
            m_dend_id = true;
            m_dtword_id = true;
            m_shift_extend_flag = true;
            m_alu_adsp_flag = true;
        }
    }

    public void mov_dsr_imm8(ushort irdata, int state)
    {
        m_dstart_id = true;
        m_dend_id = true;
        m_sel_cbus1_irl_id = true;
        m_sel_ex2_cbus1_id = true;
        m_cntl_move_id = true;
        m_sel_excom_irn = true;
        m_edsr_id = true;
        m_disint_id = true;
        m_state_clr_id = true;
        m_dtword_id = false;
        m_alu_mov_flag = true;
    }

    public void swi_imm7(ushort irdata, int state)
    {
        if (m_reset_req)
        {
            rstcycle(irdata, state);
            m_int_cycle_id = true;
        }
        else if (m_nmi_req || m_mi_req)
        {
            hardware_int(irdata, state);
            m_int_cycle_id = true;
        }
        else if (m_nmice_req)
        {
            nmice_int(irdata, state);
            m_int_cycle_id = true;
        }
        else
        {
            software_int(irdata, state);
        }
    }

    public void exe_psw_imm8(ushort irdata, int state)
    {
        min_cycle(irdata, state);
        m_sel_excom_irn = true;
        m_sel_r0_psw = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_cbus1_irl_id = true;
        m_sel_ex2_cbus1_id = true;
        m_pswbit_id = true;
        m_wr_psw_wb_id = true;
        m_dtword_id = false;
        m_alu_mov_flag = true;
        switch (irdata)
        {
            case 60680:
            case 60800:
                m_alu_psw_or_flag = true;
                m_alu_mov_flag = false;
                break;
            case 60287:
                m_alu_psw_and_flag = true;
                m_alu_mov_flag = false;
                break;
        }
    }

    public void task_branch_c16(ushort irdata, int state)
    {
        uint num = BM.I2UI(irdata & 0xF);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_csr_irn_id = true;
                m_sel_pc_pcbus_id = true;
                m_dtword_id = false;
                if (num == 1)
                {
                    m_wr_lcsr_csr_id = true;
                    m_wr_lr_nextpc_id = true;
                    m_step_inst1_flag = true;
                    if (m_StepFlag != 3)
                    {
                        m_StepOverPC = m_pc20 + 4;
                    }
                }
                break;
            case 1:
                m_pcstb_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                if (num == 1)
                {
                    m_step_inst1_flag = true;
                }
                break;
        }
    }

    public void task_branch_ern(ushort irdata, int state)
    {
        uint num = BM.I2UI(irdata & 0xF);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_greg1_entry_id = true;
                m_greg2_entry_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_dtword_id = false;
                if (num == 3)
                {
                    m_wr_lcsr_csr_id = true;
                    m_wr_lr_pc_id = true;
                    m_step_inst1_flag = true;
                    if (m_StepFlag != 3)
                    {
                        m_StepOverPC = m_pc20 + 2;
                    }
                }
                break;
            case 1:
                m_pcstb_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                if (num == 3)
                {
                    m_step_inst1_flag = true;
                }
                break;
        }
    }

    public void task_mul(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_clear_temp = true;
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_greg0_entry_id = true;
                m_greg1_entry_id = true;
                m_exe_mul_id = true;
                m_sel_temp_id = false;
                m_exe_mul_div_wb_id = false;
                m_dtword_id = false;
                m_alu_mul_flag = true;
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                mul_default();
                break;
            case 7:
                m_clear_temp = false;
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_r0_mul_div = true;
                m_greg0_entry_id = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                m_exe_mul_id = true;
                m_exe_mul_div_wb_id = true;
                m_exe_wr_z_id = true;
                m_dtword_id = false;
                m_alu_mul_flag = true;
                break;
            case 8:
                m_clear_temp = false;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_r0_mul_div = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_exe_mul_id = false;
                m_data_move_id = true;
                m_greg0_entry_id = true;
                m_exe_mul_div_wb_id = true;
                m_exe_wr_zand_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                m_alu_mul_flag = true;
                break;
        }
    }

    public void task_exe_ern_erm(ushort irdata, int state)
    {
        switch ((byte)(irdata & 0xF))
        {
            case 6:
                m_alu_add_flag = true;
                break;
            case 5:
                m_alu_mov_flag = true;
                break;
            case 7:
                m_alu_cmp_flag = true;
                break;
        }
        if (m_inst_mode == 0)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_greg0_entry_id = true;
                    m_greg1_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_sel_excom_irl = true;
                    m_exe_word1_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    break;
                case 1:
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_greg1_regm_bit0or_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_exe_word1_id = true;
                    m_sel_excom_irl = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                    break;
            }
        }
        else if (state == 0)
        {
            m_dstart_id = true;
            m_greg0_entry_id = true;
            m_greg1_entry_id = true;
            m_sel_ex1_cbus0_id = true;
            m_sel_ex2_cbus1_id = true;
            m_sel_excom_irl = true;
            m_exe_word1_id = true;
            m_dend_id = true;
            m_dtword_id = true;
        }
    }

    public void task_div(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_clear_temp = true;
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                m_greg0_entry_id = true;
                m_greg1_entry_id = true;
                m_sel_cbus1_mul_div_ex2_id = false;
                m_exe_div_id = true;
                m_exe_mul_div_wr_c_id = true;
                m_alu_div_flag = true;
                m_dtword_id = false;
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                div_default();
                break;
            case 7:
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_r0_mul_div = true;
                m_greg0_entry_id = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                m_exe_div_id = true;
                m_exe_mul_div_wb_id = true;
                m_exe_wr_z_id = true;
                m_alu_div_flag = true;
                m_dtword_id = false;
                break;
            case 8:
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_exe_div_id = true;
                m_alu_div_flag = true;
                m_dtword_id = false;
                break;
            case 15:
                m_pc_wait_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_r0_mul_div = true;
                m_greg0_entry_id = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_exe_div_id = true;
                m_exe_mul_div_wb_id = true;
                m_exe_wr_zand_id = true;
                m_alu_div_flag = true;
                m_dtword_id = false;
                break;
            case 16:
                m_sel_ex1_cbus0_id = true;
                m_sel_ex2_cbus1_id = true;
                m_sel_r0_mul_div = true;
                m_sel_cbus1_mul_div_ex2_id = true;
                m_exe_div_id = true;
                m_exe_mul_div_wb_id = true;
                m_sel_temp_id = true;
                m_greg0_entry_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_lea_erm(ushort irdata, int state)
    {
        m_lea_dirct_id = true;
        min_cycle(irdata, state);
        m_sel_a16r_abus_id = true;
        m_wr_eal_id = true;
        m_wr_eah_id = true;
        m_greg1_entry_id = true;
        m_greg2_entry_id = true;
        m_dtword_id = false;
    }

    public void task_lea_d16_erm(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_sel_a16r_abus_id = true;
                m_sel_a16l_eabus_id = true;
                m_wr_eal_id = true;
                m_wr_eah_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_lea_d16(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_lea_dirct_id = true;
                m_dstart_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_eal_id = true;
                m_wr_eah_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void mov_cprn_ea(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            ushort num2 = (ushort)(irdata & 0xF0FFu);
            switch (num2)
            {
                case 61485:
                case 61501:
                case 61517:
                case 61533:
                case 61549:
                case 61565:
                    num = 1;
                    switch (state)
                    {
                        case 0:
                            m_memory_load_ea_id = true;
                            m_cop_store_id = true;
                            m_greg0_entry_id = true;
                            m_sel_ex1_cbus0_id = true;
                            m_dstart_id = true;
                            m_sel_adbus_ea_id = true;
                            m_sel_arbus_eabus_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            m_sel_abus_bound_id = true;
                            m_dtword_id = true;
                            m_alu_mov_flag = true;
                            if (num2 == 61485 || num2 == 61501)
                            {
                                m_dend_id = true;
                                if (num2 == 61501)
                                {
                                    m_ea_plus_id = true;
                                }
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                            break;
                        case 1:
                            m_memory_load_ea_id = true;
                            m_cop_store_id = true;
                            m_greg0_entry_id = true;
                            m_sel_ex1_cbus0_id = true;
                            m_sel_greg0_regn_bit1or_id = true;
                            m_sel_a16l_eabus_id = true;
                            m_sel_a16r_1_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            if (num2 == 61517 || num2 == 61533)
                            {
                                m_dend_id = true;
                                if (num2 == 61533)
                                {
                                    m_ea_plus_id = true;
                                }
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                            m_dtword_id = true;
                            m_alu_mov_flag = true;
                            break;
                        case 2:
                            if (num2 == 61549 || num2 == 61565)
                            {
                                m_memory_load_ea_id = true;
                                m_cop_store_id = true;
                                m_greg0_entry_id = true;
                                m_sel_ex1_cbus0_id = true;
                                m_sel_greg0_regn_bit2or_id = true;
                                m_sel_a16l_eabus_id = true;
                                m_sel_a16r_1_id = true;
                                m_wr_arl_id = true;
                                m_wr_arh_id = true;
                                m_pc_wait_id = true;
                                m_dtword_id = true;
                                m_alu_mov_flag = true;
                            }
                            break;
                        case 3:
                            if (num2 == 61549 || num2 == 61565)
                            {
                                m_memory_load_ea_id = true;
                                m_cop_store_id = true;
                                m_greg0_entry_id = true;
                                m_sel_ex1_cbus0_id = true;
                                m_sel_greg0_regn_bit1or_id = true;
                                m_sel_greg0_regn_bit2or_id = true;
                                m_sel_a16l_eabus_id = true;
                                m_sel_a16r_1_id = true;
                                m_wr_arl_id = true;
                                m_wr_arh_id = true;
                                m_dend_id = true;
                                m_dtword_id = true;
                                m_alu_mov_flag = true;
                                if (num2 == 61565)
                                {
                                    m_ea_plus_id = true;
                                }
                            }
                            break;
                    }
                    break;
            }
        }
        if (num != 0)
        {
            return;
        }
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        m_memory_load_ea_id = true;
        m_cop_store_id = true;
        m_greg0_entry_id = true;
        m_sel_ex1_cbus0_id = true;
        task_register_inc(state);
        m_alu_mov_flag = true;
        switch (state)
        {
            case 0:
                m_sel_adbus_ea_id = true;
                m_sel_arbus_eabus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if ((b & 6) == 0)
                {
                    m_dend_id = true;
                    if ((b & 1) == 1)
                    {
                        m_ea_plus_id = true;
                    }
                }
                else
                {
                    m_pc_wait_id = true;
                    m_sel_abus_bound_id = true;
                }
                m_dtword_id = false;
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                m_sel_a16l_eabus_id = true;
                m_sel_a16r_1_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_dtword_id = false;
                switch (state)
                {
                    case 1:
                        if ((b & 6) == 2)
                        {
                            m_dend_id = true;
                            if ((b & 1) == 1)
                            {
                                m_ea_plus_id = true;
                            }
                        }
                        else
                        {
                            m_pc_wait_id = true;
                        }
                        break;
                    case 3:
                        if ((b & 6) == 4)
                        {
                            m_dend_id = true;
                            if ((b & 1) == 1)
                            {
                                m_ea_plus_id = true;
                            }
                        }
                        else
                        {
                            m_pc_wait_id = true;
                        }
                        break;
                    case 7:
                        m_dend_id = true;
                        if ((b & 1) == 1)
                        {
                            m_ea_plus_id = true;
                        }
                        break;
                    default:
                        m_pc_wait_id = true;
                        break;
                }
                break;
        }
    }

    public void mov_ea_cprn(ushort irdata, int state)
    {
        byte b = (byte)((irdata & 0xF0) >> 4);
        switch (state)
        {
            case 0:
                m_memory_load_ea_id = true;
                m_cop_read_id = true;
                m_memory_store_id = true;
                m_greg0_entry_id = true;
                m_sel_ex1_cbus0_id = true;
                m_dstart_id = true;
                m_sel_adbus_ea_id = true;
                m_sel_arbus_eabus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if (b == 9)
                {
                    m_ea_plus_id = true;
                }
                if (b == 8 || b == 9)
                {
                    m_dend_id = true;
                    m_dtword_id = false;
                }
                if (b != 10 && b != 11 && b != 12 && b != 13 && b != 14 && b != 15)
                {
                    break;
                }
                if (m_inst_mode == 0)
                {
                    if (b == 10 || b == 11)
                    {
                        m_pc_wait_id = true;
                    }
                    m_dtword_id = false;
                }
                else
                {
                    m_dtword_id = true;
                    if (b == 10 || b == 11)
                    {
                        m_dend_id = true;
                        if (b == 11)
                        {
                            m_ea_plus_id = true;
                        }
                    }
                }
                if (b == 12 || b == 13 || b == 14 || b == 15)
                {
                    m_pc_wait_id = true;
                }
                m_sel_abus_bound_id = true;
                break;
            case 1:
                if ((b != 10 && b != 11 && b != 12 && b != 13 && b != 14 && b != 15) || (m_inst_mode != 0 && b != 12 && b != 13 && b != 14 && b != 15))
                {
                    break;
                }
                m_memory_load_ea_id = true;
                m_cop_read_id = true;
                m_memory_store_id = true;
                m_greg0_entry_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_a16l_eabus_id = true;
                m_sel_a16r_1_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if (b == 11)
                {
                    m_ea_plus_id = true;
                }
                if (m_inst_mode == 0)
                {
                    m_sel_greg0_regn_bit0or_id = true;
                    m_dtword_id = false;
                    if (b == 10 || b == 11)
                    {
                        m_dend_id = true;
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                    break;
                }
                m_sel_greg0_regn_bit1or_id = true;
                m_dtword_id = true;
                if (b == 13)
                {
                    m_ea_plus_id = true;
                }
                if (b == 14 || b == 15)
                {
                    m_pc_wait_id = true;
                }
                else
                {
                    m_dend_id = true;
                }
                break;
            case 2:
                if ((b == 12 || b == 13 || b == 14 || b == 15) && (m_inst_mode == 0 || b == 14 || b == 15))
                {
                    m_memory_load_ea_id = true;
                    m_cop_read_id = true;
                    m_memory_store_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_pc_wait_id = true;
                    if (m_inst_mode == 0)
                    {
                        m_sel_greg0_regn_bit1or_id = true;
                        m_dtword_id = false;
                    }
                    else
                    {
                        m_sel_greg0_regn_bit2or_id = true;
                        m_dtword_id = true;
                    }
                }
                break;
            case 3:
                if ((b != 12 && b != 13 && b != 14 && b != 15) || (m_inst_mode != 0 && b != 14 && b != 15))
                {
                    break;
                }
                m_memory_load_ea_id = true;
                m_cop_read_id = true;
                m_memory_store_id = true;
                m_greg0_entry_id = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_a16l_eabus_id = true;
                m_sel_a16r_1_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                if (m_inst_mode == 0)
                {
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_greg0_regn_bit1or_id = true;
                    m_dtword_id = false;
                    if (b == 14 || b == 15)
                    {
                        m_pc_wait_id = true;
                    }
                    else
                    {
                        m_dend_id = true;
                    }
                    if (b == 13)
                    {
                        m_ea_plus_id = true;
                    }
                }
                else
                {
                    m_sel_greg0_regn_bit1or_id = true;
                    m_sel_greg0_regn_bit2or_id = true;
                    m_dend_id = true;
                    m_dtword_id = true;
                    if (b == 15)
                    {
                        m_ea_plus_id = true;
                    }
                }
                break;
            case 4:
                if ((b == 14 || b == 15) && m_inst_mode == 0)
                {
                    m_memory_load_ea_id = true;
                    m_cop_read_id = true;
                    m_memory_store_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_greg0_regn_bit2or_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                }
                break;
            case 5:
                if ((b == 14 || b == 15) && m_inst_mode == 0)
                {
                    m_memory_load_ea_id = true;
                    m_cop_read_id = true;
                    m_memory_store_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_greg0_regn_bit2or_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                }
                break;
            case 6:
                if ((b == 14 || b == 15) && m_inst_mode == 0)
                {
                    m_memory_load_ea_id = true;
                    m_cop_read_id = true;
                    m_memory_store_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_greg0_regn_bit1or_id = true;
                    m_sel_greg0_regn_bit2or_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                }
                break;
            case 7:
                if ((b == 14 || b == 15) && m_inst_mode == 0)
                {
                    m_memory_load_ea_id = true;
                    m_cop_read_id = true;
                    m_memory_store_id = true;
                    m_greg0_entry_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_greg0_regn_bit0or_id = true;
                    m_sel_greg0_regn_bit1or_id = true;
                    m_sel_greg0_regn_bit2or_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                    if (b == 15)
                    {
                        m_ea_plus_id = true;
                    }
                }
                break;
        }
    }

    public int fn_id_push_obj(ushort irdata, int state)
    {
        int result = 0;
        ushort num = BM.I2W(irdata & 0xF0FF);
        if (num == 61518 || num == 61534 || num == 61550 || num == 61566)
        {
            result = 1;
            switch (state)
            {
                case 0:
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_dstart_id = true;
                    if (num == 61566 || num == 61550)
                    {
                        m_sel_greg0_regn_bit1or_id = true;
                        if (num == 61566)
                        {
                            m_sel_greg0_regn_bit2or_id = true;
                        }
                    }
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_ff_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_greg0_entry_id = true;
                    m_dtword_id = true;
                    if (num == 61534 || num == 61518)
                    {
                        m_wr_sp_id = true;
                        m_dend_id = true;
                        if (num == 61518)
                        {
                            m_dtsize_id = false;
                        }
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                    break;
                case 1:
                    if (num == 61566 || num == 61550)
                    {
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                        if (num == 61566)
                        {
                            m_sel_greg0_regn_bit2or_id = true;
                            m_pc_wait_id = true;
                        }
                        else
                        {
                            m_wr_sp_id = true;
                            m_dend_id = true;
                        }
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_ff_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_dtword_id = true;
                    }
                    break;
                case 2:
                    if (num == 61566)
                    {
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                        m_sel_greg0_regn_bit1or_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_ff_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_pc_wait_id = true;
                        m_dtword_id = true;
                    }
                    break;
                case 3:
                    if (num == 61566)
                    {
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_ff_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_dend_id = true;
                        m_dtword_id = true;
                    }
                    break;
            }
        }
        return result;
    }

    public int fn_id_push_reglist(ushort irdata, int state)
    {
        int result = 0;
        ushort num = BM.I2W(irdata & 0xFFFF);
        switch (num)
        {
            case 61902:
                result = 1;
                if (state == 0)
                {
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_ff_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_dstart_id = true;
                    m_sel_r0_eal = true;
                    m_dend_id = true;
                    m_dtword_id = true;
                }
                break;
            case 62158:
            case 62414:
            case 63182:
            case 63438:
            case 64206:
            case 64462:
            case 65230:
            case 65486:
                result = 1;
                switch (state)
                {
                    case 0:
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_ff_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_dstart_id = true;
                        m_dtword_id = true;
                        if (m_MemoryModel == 0)
                        {
                            m_sel_r0_elrl = true;
                            m_ir_bit9_clr = true;
                            m_state_clr_id = true;
                            if (num == 62158)
                            {
                                m_dend_id = true;
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                        }
                        else
                        {
                            m_pc_wait_id = true;
                            m_sel_r0_ecsr = true;
                        }
                        break;
                    case 1:
                        if (m_MemoryModel != 0)
                        {
                            m_memory_store_id = true;
                            m_sel_ex1_cbus0_id = true;
                            m_sel_eabus_sp_id = true;
                            m_sel_a16l_eabus_id = true;
                            m_sel_a16r_ff_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            m_wr_sp_id = true;
                            m_sel_r0_elrl = true;
                            m_ir_bit9_clr = true;
                            m_state_clr_id = true;
                            m_dtword_id = true;
                            if (num == 62158)
                            {
                                m_dend_id = true;
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                        }
                        break;
                }
                break;
            case 62670:
            case 62926:
            case 64718:
            case 64974:
                result = 1;
                if (state == 0)
                {
                    m_memory_store_id = true;
                    m_sel_ex1_cbus0_id = true;
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_ff_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_dstart_id = true;
                    m_sel_r0_epsw = true;
                    m_ir_bit10_clr = true;
                    m_state_clr_id = true;
                    m_dtword_id = true;
                    if (num == 62670)
                    {
                        m_dend_id = true;
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                }
                break;
            case 63694:
            case 63950:
                result = 1;
                switch (state)
                {
                    case 0:
                        m_memory_store_id = true;
                        m_sel_ex1_cbus0_id = true;
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_ff_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_dstart_id = true;
                        m_dtword_id = true;
                        if (m_MemoryModel == 0)
                        {
                            m_sel_r0_clrl = true;
                            m_ir_bit11_clr = true;
                            m_state_clr_id = true;
                            if (num == 63694)
                            {
                                m_dend_id = true;
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                        }
                        else
                        {
                            m_pc_wait_id = true;
                            m_sel_r0_lcsr = true;
                        }
                        break;
                    case 1:
                        if (m_MemoryModel != 0)
                        {
                            m_memory_store_id = true;
                            m_sel_ex1_cbus0_id = true;
                            m_sel_eabus_sp_id = true;
                            m_sel_a16l_eabus_id = true;
                            m_sel_a16r_ff_id = true;
                            m_wr_arl_id = true;
                            m_wr_arh_id = true;
                            m_wr_sp_id = true;
                            m_sel_r0_clrl = true;
                            m_ir_bit11_clr = true;
                            m_state_clr_id = true;
                            m_dtword_id = true;
                            if (num == 63694)
                            {
                                m_dend_id = true;
                            }
                            else
                            {
                                m_pc_wait_id = true;
                            }
                        }
                        break;
                }
                break;
        }
        return result;
    }

    public int fn_id_pop_obj(ushort irdata, int state)
    {
        int result = 0;
        ushort num = BM.I2W(irdata & 0xF0FF);
        if (num == 61454 || num == 61470 || num == 61486 || num == 61502)
        {
            result = 1;
            switch (state)
            {
                case 0:
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_dstart_id = true;
                    m_greg0_entry_id = true;
                    m_memory_pop_id = true;
                    if (num == 61470 || num == 61454)
                    {
                        m_dend_id = true;
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                    if (num == 61454)
                    {
                        m_dtword_id = true;
                        m_dtsize_id = false;
                    }
                    else
                    {
                        m_dtword_id = true;
                    }
                    break;
                case 1:
                    if (num == 61486 || num == 61502)
                    {
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_sel_greg0_regn_bit1or_id = true;
                        m_greg0_entry_id = true;
                        m_memory_pop_id = true;
                        if (num == 61486)
                        {
                            m_dend_id = true;
                        }
                        else
                        {
                            m_pc_wait_id = true;
                        }
                        m_dtword_id = true;
                    }
                    break;
                case 2:
                    if (num == 61502)
                    {
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_sel_greg0_regn_bit2or_id = true;
                        m_greg0_entry_id = true;
                        m_memory_pop_id = true;
                        m_pc_wait_id = true;
                        m_dtword_id = true;
                    }
                    break;
                case 3:
                    if (num == 61502)
                    {
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_sel_greg0_regn_bit1or_id = true;
                        m_sel_greg0_regn_bit2or_id = true;
                        m_greg0_entry_id = true;
                        m_memory_pop_id = true;
                        m_dend_id = true;
                        m_dtword_id = true;
                    }
                    break;
            }
        }
        return result;
    }

    public int fn_id_pop_reglist(ushort irdata, int state)
    {
        int result = 0;
        ushort num = BM.I2W(irdata & 0xFFFF);
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if ((num & 0x100u) != 0)
        {
            result = 1;
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_memory_load_ea_id = true;
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_pop_eal_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = true;
                    break;
                case 1:
                    m_sel_cbus2_wbdata_id = true;
                    m_sel_a16r_abus_id = true;
                    m_dtword_id = true;
                    if ((b & 0xE) == 0)
                    {
                        m_dend_id = true;
                        break;
                    }
                    m_state_clr_id = true;
                    m_pc_wait_id = true;
                    m_ir_bit8_clr = true;
                    break;
            }
        }
        else if ((num & 0x800u) != 0)
        {
            result = 1;
            switch (state)
            {
                case 0:
                    m_memory_load_ea_id = true;
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_dstart_id = true;
                    m_wr_lrl_wb_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = true;
                    if (m_MemoryModel == 0)
                    {
                        if ((b & 6) == 0)
                        {
                            m_dend_id = true;
                            m_pc_wait_id = false;
                        }
                        else
                        {
                            m_state_clr_id = true;
                            m_pc_wait_id = true;
                            m_ir_bit11_clr = true;
                        }
                    }
                    break;
                case 1:
                    if (m_MemoryModel != 0)
                    {
                        m_memory_load_ea_id = true;
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_wr_lcsr_wb_id = true;
                        m_dtword_id = true;
                        if ((b & 6) == 0)
                        {
                            m_dend_id = true;
                            break;
                        }
                        m_state_clr_id = true;
                        m_pc_wait_id = true;
                        m_ir_bit11_clr = true;
                    }
                    break;
            }
        }
        else if ((num & 0x400u) != 0)
        {
            result = 1;
            if (state == 0)
            {
                m_memory_load_ea_id = true;
                m_sel_eabus_sp_id = true;
                m_sel_a16l_eabus_id = true;
                m_sel_a16r_1_id = true;
                m_sel_arbus_eabus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_wr_sp_id = true;
                m_dstart_id = true;
                m_wr_psw_wb_id = true;
                m_state_clr_id = true;
                m_dtword_id = true;
                if ((b & 2) == 0)
                {
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                    m_ir_bit10_clr = true;
                }
            }
        }
        else if ((num & 0x200u) != 0)
        {
            result = 1;
            switch (state)
            {
                case 0:
                    m_pcstb_id = true;
                    m_dstart_id = true;
                    m_memory_load_ea_id = true;
                    m_sel_eabus_sp_id = true;
                    m_sel_a16l_eabus_id = true;
                    m_sel_a16r_1_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_wr_sp_id = true;
                    m_wr_pcl_wb_id = true;
                    m_dtword_id = true;
                    m_pc_wait_id = true;
                    break;
                case 1:
                    m_pcstb_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = true;
                    if (m_MemoryModel != 0)
                    {
                        m_memory_load_ea_id = true;
                        m_sel_eabus_sp_id = true;
                        m_sel_a16l_eabus_id = true;
                        m_sel_a16r_1_id = true;
                        m_sel_arbus_eabus_id = true;
                        m_wr_arl_id = true;
                        m_wr_arh_id = true;
                        m_wr_sp_id = true;
                        m_wr_csr_wb_id = true;
                    }
                    break;
                case 2:
                    m_pcstb_id = true;
                    m_dtword_id = true;
                    if (m_MemoryModel == 0)
                    {
                        m_dend_id = true;
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                    break;
                case 3:
                    if (m_MemoryModel != 0)
                    {
                        m_pcstb_id = true;
                        m_dend_id = true;
                        m_dtword_id = true;
                    }
                    break;
            }
        }
        return result;
    }

    public void task_push_pop(ushort irdata, int state)
    {
        int num = 0;
        if (m_inst_mode != 0)
        {
            switch (BM.I2W(irdata & 0xF0CF))
            {
                case 61518:
                    if (fn_id_push_obj(irdata, state) == 1)
                    {
                        num = 1;
                    }
                    break;
                case 61646:
                    if (fn_id_push_reglist(irdata, state) == 1)
                    {
                        num = 1;
                    }
                    break;
                case 61454:
                    if (fn_id_pop_obj(irdata, state) == 1)
                    {
                        num = 1;
                    }
                    break;
                case 61582:
                    if (fn_id_pop_reglist(irdata, state) == 1)
                    {
                        num = 1;
                    }
                    break;
            }
        }
        if (num != 0)
        {
            return;
        }
        byte num2 = (byte)((irdata >> 4) & 0xF);
        byte b = (byte)(num2 & 0xCu);
        byte b2 = (byte)(num2 & 3u);
        switch (b)
        {
            case 4:
                if (b2 != 0 || state != 0)
                {
                    m_memory_store_id = true;
                }
                m_sel_ex1_cbus0_id = true;
                switch (b2)
                {
                    case 3:
                        task_qreg_dec(irdata, state);
                        break;
                    case 2:
                        task_xreg_dec(irdata, state);
                        break;
                    case 1:
                        task_ereg_dec(irdata, state);
                        break;
                    default:
                        m_greg0_entry_id = true;
                        break;
                }
                task_push_reg(irdata, state);
                break;
            case 0:
                task_set_a16_sp_1(irdata, state);
                m_sel_arbus_eabus_id = true;
                m_wr_arl_id = true;
                m_wr_arh_id = true;
                m_wr_sp_id = true;
                task_register_inc(state);
                if (b2 != 0 || state == 0)
                {
                    m_greg0_entry_id = true;
                    m_memory_pop_id = true;
                }
                task_pop_reg(irdata, state);
                break;
            case 12:
                m_memory_store_id = true;
                m_sel_ex1_cbus0_id = true;
                task_sel_push_cntl(irdata, state);
                break;
            default:
                task_sel_pop_cntl(irdata, state);
                break;
        }
    }

    public void task_rt_rti(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        switch (state)
        {
            case 0:
                m_sel_excom_irm = true;
                m_dstart_id = true;
                m_sel_csr_cbus0_id = true;
                m_sel_pc_pcbus_id = true;
                m_sel_a16r_abus_id = true;
                m_dtword_id = false;
                if (b == 0)
                {
                    m_sel_cbus1_elrl_id = true;
                    m_sel_cbus2_elrh_id = true;
                    m_sel_r0_ecsr = true;
                    m_wr_psw_epsw_id = true;
                }
                else
                {
                    m_sel_cbus1_clrl_id = true;
                    m_sel_cbus2_clrh_id = true;
                    m_sel_r0_lcsr = true;
                    m_step_inst2_flag = true;
                }
                break;
            case 1:
                m_sel_excom_irm = true;
                m_pcstb_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                if (b == 1)
                {
                    m_step_inst2_flag = true;
                }
                break;
        }
    }

    public void inc_dec_ea(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        if ((uint)(b - 2) <= 1u)
        {
            switch (state)
            {
                case 0:
                    m_sel_excom_irm = true;
                    m_dtlock_id = true;
                    m_dstart_id = true;
                    m_sel_adbus_ea_id = true;
                    m_sel_arbus_eabus_id = true;
                    m_wr_arl_id = true;
                    m_wr_arh_id = true;
                    m_memory_load_ea_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    break;
                case 1:
                    m_sel_excom_irm = true;
                    m_dend_id = true;
                    m_sel_adbus_ea_id = true;
                    m_memory_store_id = true;
                    m_sel_ex2_cbus1_id = true;
                    m_incdecea_id = true;
                    m_sel_cbus1_const1_id = true;
                    m_dtword_id = false;
                    if (b == 2)
                    {
                        m_alu_inc_flag = true;
                    }
                    else
                    {
                        m_alu_dec_flag = true;
                    }
                    m_wr_ex1_wb_id = true;
                    break;
            }
        }
        else
        {
            undefine_inst(irdata, state);
        }
    }

    public void task_rtice(ushort irdata, int state)
    {
        uint num = BM.I2UI((irdata >> 4) & 0xF);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus1_elrl_id = true;
                m_sel_cbus2_elrh_id = true;
                m_sel_r0_ecsr = true;
                m_sel_csr_cbus0_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_dtword_id = false;
                if (num == 6)
                {
                    m_tbl_ffef_id = true;
                    m_dend_id = true;
                }
                else
                {
                    m_wr_psw_epsw_id = true;
                }
                break;
            case 1:
                if (num == 7)
                {
                    m_pcstb_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                }
                break;
            case 2:
                if (num == 7)
                {
                    m_pcstb_id = true;
                    m_dend_id = true;
                    m_dtword_id = false;
                }
                break;
        }
    }

    public void min_cycle(ushort irdata, int state)
    {
        m_dstart_id = true;
        m_dend_id = true;
        m_dtword_id = false;
    }

    public void undefine_inst(ushort irdata, int state)
    {
        m_SimRunFlag = 3;
        min_cycle(irdata, state);
    }

    public void exe_di_com(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_excom_irn = true;
                m_sel_r0_psw = true;
                m_sel_ex1_cbus0_id = true;
                m_sel_cbus1_irl_id = true;
                m_sel_ex2_cbus1_id = true;
                m_pswbit_id = true;
                m_wr_psw_wb_id = true;
                m_pc_wait_id = true;
                m_alu_psw_and_flag = true;
                m_dtword_id = false;
                break;
            case 1:
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 2:
                m_dend_id = true;
                m_dtword_id = false;
                m_inst_DI = true;
                break;
        }
    }

    public void iceswi(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_pc_clear_id = true;
                m_sel_csr_clr_id = true;
                m_wr_intecsr_oldcsr_id = true;
                m_wr_intelr_oldpc_id = true;
                m_iceswi_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_dtword_id = false;
                break;
            case 2:
                m_dtword_id = false;
                break;
            case 3:
                m_dtword_id = false;
                break;
            case 4:
                m_intack_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 5:
                m_pcstb_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_sel_psw_nmice_id = true;
                m_dtword_id = false;
                break;
            case 6:
                m_pcstb_id = true;
                m_disint_user_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void table_ffxf(ushort irdata, int state)
    {
        byte pSW = m_Reg.GetPSW(54);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_brk_id = true;
                m_dtword_id = false;
                m_pc_wait_id = true;
                break;
            case 2:
                if (pSW < 2)
                {
                    m_sel_csr_clr_id = true;
                    m_wr_intecsr_csr_id = true;
                    m_wr_intelr_pc_id = true;
                }
                m_brkswi_id = true;
                m_dtword_id = false;
                m_brk_id = true;
                break;
            case 3:
                if (pSW >= 2)
                {
                    m_sel_cbus1_roml_id = true;
                    m_sel_cbus2_romh_id = true;
                    m_sel_a16r_abus_id = true;
                    m_wr_sp_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                m_brk_id = true;
                break;
            case 4:
                if (pSW >= 2)
                {
                    m_brkswi_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                m_brk_id = true;
                break;
            case 5:
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                if (pSW >= 2)
                {
                    m_brkswi_id = true;
                }
                else
                {
                    m_sel_psw_nmi_id = true;
                }
                m_dtword_id = false;
                break;
            case 6:
                m_disint_user_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_a16l_ar()
    {
        m_sel_a16l_eabus_id = true;
    }

    public void task_inc_eabus()
    {
        task_a16l_ar();
        m_sel_a16r_1_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
    }

    public void task_add_ar_abus()
    {
        task_a16l_ar();
        m_sel_a16r_abus_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
    }

    public void m_task_inc_eabus()
    {
        task_a16l_ar();
        m_sel_a16r_1_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
    }

    public void task_set_sp_ar()
    {
        m_sel_eabus_sp_id = true;
        m_sel_a16l_eabus_id = true;
        m_wr_sp_id = true;
    }

    public void mul_default()
    {
        m_pc_wait_id = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_ex2_cbus1_id = true;
        m_sel_r0_mul_div = true;
        m_sel_cbus1_mul_div_ex2_id = true;
        m_exe_mul_id = true;
        m_alu_mul_flag = true;
        m_dtword_id = false;
    }

    public void div_default()
    {
        m_pc_wait_id = true;
        m_sel_ex1_cbus0_id = true;
        m_sel_ex2_cbus1_id = true;
        m_sel_r0_mul_div = true;
        m_sel_cbus1_mul_div_ex2_id = true;
        m_exe_div_id = true;
        m_alu_div_flag = true;
        m_dtword_id = false;
    }

    public void task_register_inc(int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                break;
            case 1:
                m_sel_greg0_regn_bit0or_id = true;
                break;
            case 2:
                m_sel_greg0_regn_bit1or_id = true;
                break;
            case 3:
                m_sel_greg0_regn_bit0or_id = true;
                m_sel_greg0_regn_bit1or_id = true;
                break;
            case 4:
                m_sel_greg0_regn_bit2or_id = true;
                break;
            case 5:
                m_sel_greg0_regn_bit0or_id = true;
                m_sel_greg0_regn_bit2or_id = true;
                break;
            case 6:
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit2or_id = true;
                break;
            case 7:
                m_sel_greg0_regn_bit0or_id = true;
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit2or_id = true;
                break;
        }
    }

    public void task_qreg_dec(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit2or_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit2or_id = true;
                break;
            case 2:
                m_sel_greg0_regn_bit2or_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                break;
            case 3:
                m_sel_greg0_regn_bit2or_id = true;
                break;
            case 4:
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                break;
            case 5:
                m_sel_greg0_regn_bit1or_id = true;
                break;
            case 6:
                m_sel_greg0_regn_bit0or_id = true;
                break;
        }
    }

    public void task_xreg_dec(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_sel_greg0_regn_bit1or_id = true;
                m_sel_greg0_regn_bit0or_id = true;
                break;
            case 1:
                m_sel_greg0_regn_bit1or_id = true;
                break;
            case 2:
                m_sel_greg0_regn_bit0or_id = true;
                break;
        }
    }

    public void task_ereg_dec(ushort irdata, int state)
    {
        if (state == 0)
        {
            m_sel_greg0_regn_bit0or_id = true;
        }
    }

    public void task_push_reg(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            task_set_a16_sp_ffff();
            m_wr_arl_id = true;
            m_wr_arh_id = true;
            m_pc_wait_id = true;
            m_greg0_entry_id = true;
            m_dtword_id = false;
            return;
        }
        task_dec_ar();
        switch (state)
        {
            case 1:
                if ((b & 2) == 0)
                {
                    m_wr_sp_id = true;
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                break;
            case 3:
                if ((b & 1) == 0)
                {
                    m_wr_sp_id = true;
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                break;
            case 7:
                m_wr_sp_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
            default:
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_set_a16_sp_1(ushort irdata, int state)
    {
        m_sel_eabus_sp_id = true;
        m_sel_a16l_eabus_id = true;
        m_sel_a16r_1_id = true;
    }

    public void task_pop_reg(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 4) & 0xFu);
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 1:
                if ((b & 2) == 0)
                {
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                m_dtword_id = false;
                break;
            case 3:
                m_dtword_id = false;
                if ((b & 1) == 0)
                {
                    m_dend_id = true;
                }
                else
                {
                    m_pc_wait_id = true;
                }
                break;
            case 7:
                m_dend_id = true;
                m_dtword_id = false;
                break;
            default:
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void task_sel_push_cntl(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if (b == 0)
        {
            min_cycle(irdata, state);
            return;
        }
        task_set_a16_sp_ffff();
        m_wr_arl_id = true;
        m_wr_arh_id = true;
        m_wr_sp_id = true;
        if ((b & 2) == 2)
        {
            task_push_elr(irdata, state);
        }
        else if ((b & 6) == 4)
        {
            task_push_epsw(irdata, state);
        }
        else if ((b & 0xE) == 8)
        {
            task_push_lr(irdata, state);
        }
        else
        {
            task_push_ea(irdata, state);
        }
    }

    public void task_sel_pop_cntl(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if ((b & 1) == 1)
        {
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    task_sp_inc(irdata, state);
                    m_pop_eal_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    return;
                case 1:
                    m_sel_cbus1_wbdata_id = true;
                    m_sel_a16r_abus_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    return;
                case 2:
                    m_pop_eah_id = true;
                    task_sp_inc(irdata, state);
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    return;
            }
            m_sel_cbus2_wbdata_id = true;
            m_sel_a16r_abus_id = true;
            m_dtword_id = false;
            if ((b & 0xE) == 0)
            {
                m_dend_id = true;
                return;
            }
            m_state_clr_id = true;
            m_pc_wait_id = true;
            m_ir_bit8_clr = true;
        }
        else if ((b & 9) == 8)
        {
            task_sp_inc(irdata, state);
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    m_wr_lrl_wb_id = true;
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    return;
                case 1:
                    m_wr_lrl_wb_id = true;
                    m_dtword_id = false;
                    if (m_MemoryModel == 0)
                    {
                        if ((b & 6) == 0)
                        {
                            m_dend_id = true;
                            return;
                        }
                        m_state_clr_id = true;
                        m_pc_wait_id = true;
                        m_ir_bit11_clr = true;
                    }
                    else
                    {
                        m_pc_wait_id = true;
                    }
                    return;
                case 2:
                    if (m_MemoryModel != 0)
                    {
                        m_wr_lcsr_wb_id = true;
                        m_pc_wait_id = true;
                        m_dtword_id = false;
                    }
                    return;
            }
            if (m_MemoryModel != 0)
            {
                m_dtword_id = false;
                if ((b & 6) == 0)
                {
                    m_dend_id = true;
                    return;
                }
                m_state_clr_id = true;
                m_pc_wait_id = true;
                m_ir_bit11_clr = true;
            }
        }
        else if ((b & 0xD) == 4)
        {
            task_sp_inc(irdata, state);
            if (state == 0)
            {
                m_dstart_id = true;
                m_wr_psw_wb_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
            }
            m_state_clr_id = true;
            m_dtword_id = false;
            if ((b & 2) == 0)
            {
                m_dend_id = true;
                return;
            }
            m_pc_wait_id = true;
            m_ir_bit10_clr = true;
        }
        else if (b == 2)
        {
            m_pcstb_id = true;
            switch (state)
            {
                case 0:
                    m_dstart_id = true;
                    task_sp_inc(irdata, state);
                    m_wr_pcl_wb_id = true;
                    m_dtword_id = false;
                    break;
                case 1:
                    m_wr_pcl_wb_id = true;
                    task_sp_inc(irdata, state);
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    break;
                case 2:
                    m_pc_wait_id = true;
                    m_dtword_id = false;
                    if (m_MemoryModel != 0)
                    {
                        task_sp_inc(irdata, state);
                        m_wr_csr_wb_id = true;
                    }
                    break;
                case 3:
                    m_dtword_id = false;
                    if (m_MemoryModel == 0)
                    {
                        m_dend_id = true;
                        break;
                    }
                    task_sp_inc(irdata, state);
                    m_pc_wait_id = true;
                    break;
                default:
                    if (m_MemoryModel != 0)
                    {
                        m_dend_id = true;
                        m_dtword_id = false;
                    }
                    break;
            }
        }
        else
        {
            min_cycle(irdata, state);
        }
    }

    public void task_set_a16_sp_ffff()
    {
        m_sel_eabus_sp_id = true;
        m_sel_a16l_eabus_id = true;
        m_sel_a16r_ff_id = true;
    }

    public void task_dec_ar()
    {
        task_a16l_ar();
        m_sel_a16r_ff_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
    }

    public void task_push_elr(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if (m_MemoryModel == 0)
        {
            if (state == 0)
            {
                m_dstart_id = true;
                m_sel_r0_elrh = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
            }
            m_sel_r0_elrl = true;
            m_ir_bit9_clr = true;
            m_state_clr_id = true;
            m_dtword_id = false;
            if ((b & 0xD) == 0)
            {
                m_dend_id = true;
            }
            else
            {
                m_pc_wait_id = true;
            }
            return;
        }
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                m_neg_id = true;
                return;
            case 1:
                m_sel_r0_ecsr = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
            case 2:
                m_sel_r0_elrh = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
        }
        m_sel_r0_elrl = true;
        m_ir_bit9_clr = true;
        m_state_clr_id = true;
        m_dtword_id = false;
        if ((b & 0xD) == 0)
        {
            m_dend_id = true;
        }
        else
        {
            m_pc_wait_id = true;
        }
    }

    public void task_push_epsw(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if (state == 0)
        {
            m_dstart_id = true;
            m_pc_wait_id = true;
            m_dtword_id = false;
            m_neg_id = true;
            return;
        }
        m_sel_r0_epsw = true;
        m_ir_bit10_clr = true;
        m_state_clr_id = true;
        m_dtword_id = false;
        if ((b & 9) == 0)
        {
            m_dend_id = true;
        }
        else
        {
            m_pc_wait_id = true;
        }
    }

    public void task_push_lr(ushort irdata, int state)
    {
        byte b = (byte)((uint)(irdata >> 8) & 0xFu);
        if (m_MemoryModel == 0)
        {
            if (state == 0)
            {
                m_dstart_id = true;
                m_sel_r0_clrh = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
            }
            m_sel_r0_clrl = true;
            m_ir_bit11_clr = true;
            m_state_clr_id = true;
            m_dtword_id = false;
            if ((b & 1) == 0)
            {
                m_dend_id = true;
            }
            else
            {
                m_pc_wait_id = true;
            }
            return;
        }
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                m_neg_id = true;
                return;
            case 1:
                m_sel_r0_lcsr = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
            case 2:
                m_sel_r0_clrh = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                return;
        }
        m_sel_r0_clrl = true;
        m_ir_bit11_clr = true;
        m_state_clr_id = true;
        m_dtword_id = false;
        if ((b & 1) == 0)
        {
            m_dend_id = true;
        }
        else
        {
            m_pc_wait_id = true;
        }
    }

    public void task_push_ea(ushort irdata, int state)
    {
        if (state == 0)
        {
            m_dstart_id = true;
            m_sel_r0_eah = true;
            m_pc_wait_id = true;
            m_dtword_id = false;
        }
        else
        {
            m_sel_r0_eal = true;
            m_dend_id = true;
            m_dtword_id = false;
        }
    }

    public void task_sp_inc(ushort irdata, int state)
    {
        m_memory_load_ea_id = true;
        task_set_a16_sp_1(irdata, state);
        m_sel_arbus_eabus_id = true;
        m_wr_arl_id = true;
        m_wr_arh_id = true;
        m_wr_sp_id = true;
    }

    public void rstcycle(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_wr_sp_id = true;
                m_dtword_id = false;
                break;
            case 2:
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_pcstb_id = true;
                m_dtword_id = false;
                break;
            case 3:
                m_resend_id = true;
                m_pcstb_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void hardware_int(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus2_irl_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_swivec_id = true;
                m_sel_csr_clr_id = true;
                m_wr_intecsr_csr_id = true;
                m_wr_intelr_pc_id = true;
                m_intack_id = true;
                m_dtword_id = true;
                break;
            case 1:
                m_pcstb_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                if (m_nmi_req)
                {
                    m_sel_psw_nmi_id = true;
                }
                else
                {
                    m_sel_psw_mi_id = true;
                }
                m_dtword_id = true;
                break;
            case 2:
                m_pcstb_id = true;
                m_disint_user_id = true;
                m_dend_id = true;
                m_dtword_id = true;
                break;
        }
    }

    public void nmice_int(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_sel_cbus2_irl_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_swivec_id = true;
                m_sel_csr_clr_id = true;
                m_wr_intecsr_csr_id = true;
                m_wr_intelr_pc_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_intack_id = true;
                m_pc_wait_id = true;
                m_dtword_id = false;
                break;
            case 2:
                m_pcstb_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_sel_psw_nmice_id = true;
                m_dtword_id = false;
                break;
            case 3:
                m_pcstb_id = true;
                m_disint_user_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void software_int(ushort irdata, int state)
    {
        switch (state)
        {
            case 0:
                m_dstart_id = true;
                m_s1_swi_id = true;
                m_sel_cbus1_const1_id = true;
                m_sel_cbus2_irl_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_swivec_id = true;
                m_wr_ecsr_csr_id = true;
                m_sel_csr_clr_id = true;
                m_wr_elr_pc_id = true;
                m_dtword_id = false;
                break;
            case 1:
                m_pcstb_id = true;
                m_sel_psw_mi_id = true;
                m_sel_cbus1_roml_id = true;
                m_sel_cbus2_romh_id = true;
                m_sel_a16r_abus_id = true;
                m_sel_pc_pcbus_id = true;
                m_dtword_id = false;
                break;
            case 2:
                m_pcstb_id = true;
                m_disint_user_id = true;
                m_dend_id = true;
                m_dtword_id = false;
                break;
        }
    }

    public void m_IdentfyInstLST(ushort irdata)
    {
        int num = (irdata >> 1) & 3;
        int num2 = (irdata >> 4) & 7;
        m_load_mode = false;
        m_byte_mode = false;
        m_word_mode = false;
        m_dword_mode = false;
        m_eaplus_mode = false;
        if ((irdata & 1) == 0)
        {
            m_load_mode = true;
        }
        switch (num)
        {
            case 0:
                m_byte_mode = true;
                break;
            case 1:
                m_word_mode = true;
                break;
            case 2:
                m_dword_mode = true;
                break;
        }
        if (num2 == 5)
        {
            m_eaplus_mode = true;
        }
    }

    public void m_ClearIdSetFlg()
    {
        m_dstart_id = false;
        m_dend_id = false;
        m_dtword_id = false;
        m_bcc_id = false;
        m_brk_id = false;
        m_brkswi_id = false;
        m_clear_temp = false;
        m_cntl_axxx_id = false;
        m_cop_read_id = false;
        m_cop_store_id = false;
        m_data_move_id = false;
        m_disint_user_id = false;
        m_dtlock_id = false;
        m_ea_mode_id = false;
        m_ea_plus_id = false;
        m_exe_alu_id = false;
        m_exe_div_id = false;
        m_exe_mul_div_wb_id = false;
        m_exe_mul_div_wr_c_id = false;
        m_exe_mul_id = false;
        m_exe_rn_rm_id = false;
        m_exe_word1_id = false;
        m_exe_word3_id = false;
        m_exe_wr_z_id = false;
        m_exe_wr_zand_id = false;
        m_exebit_id = false;
        m_extend_id = false;
        m_greg0_entry_id = false;
        m_greg1_entry_id = false;
        m_greg2_entry_id = false;
        m_incdecea_id = false;
        m_intack_id = false;
        m_ir_bit10_clr = false;
        m_ir_bit11_clr = false;
        m_ir_bit8_clr = false;
        m_ir_bit9_clr = false;
        m_lea_dirct_id = false;
        m_left_id = false;
        m_memory_load_ea_id = false;
        m_memory_load_id = false;
        m_memory_pop_id = false;
        m_memory_read_id = false;
        m_memory_store_id = false;
        m_neg_id = false;
        m_pc_clear_id = false;
        m_pc_wait_id = false;
        m_pcl_wait_id = false;
        m_pcstb_id = false;
        m_pop_eah_id = false;
        m_pop_eal_id = false;
        m_pswbit_id = false;
        m_resend_id = false;
        m_right_id = false;
        m_s1_swi_id = false;
        m_sel_a16l_eabus_id = false;
        m_sel_a16l_pc_id = false;
        m_sel_a16r_1_id = false;
        m_sel_a16r_abus_id = false;
        m_sel_a16r_ff_id = false;
        m_sel_abus_bound_id = false;
        m_sel_abus_swap_id = false;
        m_sel_abus_width_id = false;
        m_sel_adbus_ea_id = false;
        m_sel_arbus_eabus_id = false;
        m_sel_cbus1_bcc_id = false;
        m_sel_cbus1_borrow_id = false;
        m_sel_cbus1_clrl_id = false;
        m_sel_cbus1_const1_id = false;
        m_sel_cbus1_daa_id = false;
        m_sel_cbus1_ecsr_id = false;
        m_sel_cbus1_elrh_id = false;
        m_sel_cbus1_elrl_id = false;
        m_sel_cbus1_epsw_id = false;
        m_sel_cbus1_irl_id = false;
        m_sel_cbus1_mul_div_ex2_id = false;
        m_sel_cbus1_psw_id = false;
        m_sel_cbus1_roml_id = false;
        m_sel_cbus1_sph_id = false;
        m_sel_cbus1_spl_id = false;
        m_sel_cbus1_wbdata_id = false;
        m_sel_cbus2_clrh_id = false;
        m_sel_cbus2_elrh_id = false;
        m_sel_cbus2_irl_id = false;
        m_sel_cbus2_romh_id = false;
        m_sel_cbus2_wbdata_id = false;
        m_sel_csr_cbus0_id = false;
        m_sel_csr_clr_id = false;
        m_sel_csr_irn_id = false;
        m_sel_eabus_sp_id = false;
        m_sel_ex1_cbus0_id = false;
        m_sel_ex2_cbus1_id = false;
        m_sel_excom_irh = false;
        m_sel_excom_irl = false;
        m_sel_excom_irm = false;
        m_sel_excom_irn = false;
        m_sel_greg0_bp_id = false;
        m_sel_greg0_fp_id = false;
        m_sel_greg0_regn_bit0or_id = false;
        m_sel_greg0_regn_bit1or_id = false;
        m_sel_greg0_regn_bit2or_id = false;
        m_sel_greg1_regm_bit0or_id = false;
        m_sel_pc_pcbus_id = false;
        m_sel_pc_swivec_id = false;
        m_sel_psw_mi_id = false;
        m_sel_psw_nmi_id = false;
        m_sel_psw_nmice_id = false;
        m_sel_r0_clrh = false;
        m_sel_r0_clrl = false;
        m_sel_r0_eah = false;
        m_sel_r0_eal = false;
        m_sel_r0_ecsr = false;
        m_sel_r0_elrh = false;
        m_sel_r0_elrl = false;
        m_sel_r0_epsw = false;
        m_sel_r0_irl = false;
        m_sel_r0_lcsr = false;
        m_sel_r0_mul_div = false;
        m_sel_r0_psw = false;
        m_sel_temp_id = false;
        m_shift_id = false;
        m_state_clr_id = false;
        m_tbl_ffef_id = false;
        m_wr_arh_id = false;
        m_wr_arl_id = false;
        m_wr_csr_wb_id = false;
        m_wr_disp6_id = false;
        m_wr_eah_id = false;
        m_wr_eal_id = false;
        m_wr_ecsr_csr_id = false;
        m_wr_elr_pc_id = false;
        m_wr_elrl_wb_id = false;
        m_wr_ex1_wb_id = false;
        m_wr_intecsr_csr_id = false;
        m_wr_intelr_pc_id = false;
        m_wr_intecsr_oldcsr_id = false;
        m_wr_intelr_oldpc_id = false;
        m_wr_iceswi_id = false;
        m_wr_lcsr_csr_id = false;
        m_wr_lcsr_wb_id = false;
        m_wr_lr_nextpc_id = false;
        m_wr_lrl_wb_id = false;
        m_wr_pcl_wb_id = false;
        m_wr_psw_epsw_id = false;
        m_wr_psw_wb_id = false;
        m_wr_sp_id = false;
        m_edsr_id = false;
        m_disint_id = false;
        m_cntl_move_id = false;
        m_wr_lcsr_csr_id = false;
        m_wr_lr_pc_id = false;
        m_iceswi_id = false;
        m_wr_greg_wb_id = false;
        m_alu_cpc_flag = false;
        m_alu_add_flag = false;
        m_alu_adc_flag = false;
        m_alu_sub_flag = false;
        m_alu_sbc_flag = false;
        m_alu_mov_flag = false;
        m_alu_and_flag = false;
        m_alu_or_flag = false;
        m_alu_xor_flag = false;
        m_alu_rb_flag = false;
        m_alu_sb_flag = false;
        m_alu_daa_flag = false;
        m_alu_das_flag = false;
        m_alu_mul_flag = false;
        m_alu_div_flag = false;
        m_alu_cmp_flag = false;
        m_alu_reverse_flag = false;
        m_alu_inc_flag = false;
        m_alu_dec_flag = false;
        m_alu_adsp_flag = false;
        m_shift_imm7_flag = false;
        m_shift_signextend_flag = false;
        m_shift_extend_flag = false;
        m_shift_sra_flag = false;
        m_shift_srlc_flag = false;
        m_shift_sllc_flag = false;
        m_shift_sll_flag = false;
        m_shift_srl_flag = false;
        m_step_inst1_flag = false;
        m_step_inst2_flag = false;
        m_alu_psw_or_flag = false;
        m_alu_psw_and_flag = false;
        m_bcctrue = false;
        if (m_cpu_mode == 0)
        {
            m_dtsize_id = false;
        }
        else
        {
            m_dtsize_id = true;
        }
        m_inst_DI = false;
        m_int_cycle_id = false;
    }

    public void m_ClearExSetFlg()
    {
        if (m_dend_ex && m_GetDsrPrefix() == 0)
        {
            m_dsr_prifix_inst_ex = false;
        }
        m_dstart_ex = false;
        if (!m_ext_rwinsel && !m_ext_dtwait)
        {
            m_dend_ex = false;
        }
        m_pc_wait_ex = false;
        m_int_cycle_ex = false;
        m_SetDsrPrefix(0);
    }

    public void m_ExternalRun()
    {
        if (DllNum_SimPeri == 0)
        {
            return;
        }
        for (uint num = 0u; num < DllNum_SimPeri; num++)
        {
            if (hSimPeripheralDLLInst[num] != null)
            {
                periApp = hSimPeripheralDLLInst[num];
                periApp.perApi_Run();
            }
        }
    }

    public void m_InternalRun()
    {
        CSimCoproApp?.cpApi_Run();
    }

    public void m_ExternalReset()
    {
        CSimCoproApp?.cpApi_Reset();
        if (DllNum_SimPeri != 0)
        {
            for (uint num = 0u; num < DllNum_SimPeri; num++)
            {
                periApp = hSimPeripheralDLLInst[num];
                periApp.perApi_Reset();
            }
        }
    }

    public void m_GetExternalWait()
    {
        m_ext_dtwait = false;
        m_ext_pcwait = false;
        if (!string.IsNullOrEmpty(DllCopro_name))
        {
            byte b = 0;
            CSimCoproApp?.cpApi_GetWaitReq(ref b);
            if (((uint)b & (true ? 1u : 0u)) != 0)
            {
                m_ext_dtwait = true;
            }
            if ((b & 2u) != 0)
            {
                m_ext_pcwait = true;
            }
            if ((b & 4u) != 0)
            {
                m_ext_rwinsel = true;
                m_rwin_wait_counter++;
            }
        }
        if (DllNum_SimPeri == 0)
        {
            return;
        }
        byte b2 = 0;
        for (uint num = 0u; num < DllNum_SimPeri; num++)
        {
            periApp = hSimPeripheralDLLInst[num];
            periApp.perApi_GetWaitReq(ref b2);
            if (((uint)b2 & (true ? 1u : 0u)) != 0)
            {
                m_ext_dtwait = true;
            }
            if ((b2 & 2u) != 0)
            {
                m_ext_pcwait = true;
            }
            if ((b2 & 4u) != 0)
            {
                m_ext_rwinsel = true;
            }
        }
    }

    public void m_PcChange()
    {
        m_PcChanged = true;
    }

    public bool m_GetIdEnd()
    {
        return m_dend_id;
    }

    private static void exit(int code)
    {
        Environment.Exit(code);
    }

    private static void AfxMessageBox(string mes)
    {
    }

    private static bool isxdigit(char ch)
    {
        if (('0' > ch || ch > '9') && ('a' > ch || ch > 'f'))
        {
            if ('A' <= ch)
            {
                return ch <= 'F';
            }
            return false;
        }
        return true;
    }

    private static bool isdigit(char ch)
    {
        if ('0' <= ch)
        {
            return ch <= '9';
        }
        return false;
    }

    private static char toupper(char ch)
    {
        if ('a' <= ch && ch <= 'z')
        {
            return (char)(ch + -32);
        }
        return ch;
    }

    public void m_ChangeCPUMode()
    {
        byte b = 0;
        CSimMemApp.memApi_GetVal(12305, m_STPACP, ref b);
        if (b != 0)
        {
            if ((b & 0xF0) == 80)
            {
                m_STPACPflag = 1;
            }
            else if ((b & 0xF0) == 160 && m_STPACPflag == 1)
            {
                m_STPACPflag = 2;
            }
            else
            {
                m_STPACPflag = 0;
            }
            CSimMemApp.memApi_SetVal(12305, m_STPACP, 0);
        }
        if (m_reset_req || m_nmice_req || m_nmi_req || m_mi_req || m_swi_req || m_SimRunFlag == 16)
        {
            return;
        }
        CSimMemApp.memApi_GetVal(12305, m_SBYCON, ref b);
        byte b2 = (byte)(b & (byte)(m_Stop_Jdg_bit_mask | m_StopD_Jdg_bit_mask | m_Halt_Jdg_bit_mask | m_HaltH_Jdg_bit_mask | m_HaltC_Jdg_bit_mask | m_StopSV1_Jdg_bit_mask | m_HaltSV1_Jdg_bit_mask | m_StopSV2_Jdg_bit_mask));
        if (b2 == 0)
        {
            return;
        }
        if (b2 == m_Stop_Jdg_val || b2 == m_StopD_Jdg_val || b2 == m_StopSV1_Jdg_val || b2 == m_StopSV2_Jdg_val)
        {
            if (m_STPACPflag != 2)
            {
                CSimMemApp.memApi_SetVal(12305, m_SBYCON, 0);
            }
            else
            {
                m_SimRunFlag = 2;
                if (m_Logflag == 2)
                {
                    string line = "----- Stop Mode -----\n";
                    try
                    {
                        f1.WriteString(line);
                    }
                    catch (IOException)
                    {
                        f1.Abort();
                        exit(1);
                    }
                }
            }
            m_STPACPflag = 0;
        }
        else if (b2 == m_Halt_Jdg_val || b2 == m_HaltH_Jdg_val || b2 == m_HaltC_Jdg_val || b2 == m_HaltSV1_Jdg_val)
        {
            m_SimRunFlag = 1;
            if (m_Logflag == 2)
            {
                string line2 = "----- Halt Mode -----\n";
                try
                {
                    f1.WriteString(line2);
                }
                catch (IOException)
                {
                    f1.Abort();
                    exit(1);
                }
            }
        }
        else
        {
            m_ClearSBYCON();
            m_STPACPflag = 0;
        }
    }

    public void m_ClearSBYCON()
    {
        byte b = 0;
        CSimMemApp.memApi_GetVal(12305, m_SBYCON, ref b);
        b &= (byte)(~(m_Halt_Jdg_bit_mask | m_Stop_Jdg_bit_mask | m_HaltH_Jdg_bit_mask | m_StopD_Jdg_bit_mask | m_HaltC_Jdg_bit_mask | m_StopSV1_Jdg_bit_mask | m_HaltSV1_Jdg_bit_mask | m_StopSV2_Jdg_bit_mask));
        CSimMemApp.memApi_SetVal(12305, m_SBYCON, b);
    }
}
