namespace OscilloscopeConnection
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    public struct WD_PARAM
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]
        public string descriptor_name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]
        public string template_name;
        public short comm_type;
        public short comm_order;
        public int wave_desc_length;
        public int user_text_length;
        public int res_desc1;
        public int trig_time_array;
        public int ris_time_array;
        public int res_array1;
        public int wave_array_1;
        public int wave_array_2;
        public int res_array2;
        public int res_array3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]
        public string instrument_name;
        public uint instrument_number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]
        public string trace_label;
        public int reserved_data_count;
        public int wave_array_count;
        public int points_per_screen;
        public int first_valid;
        public int last_valid;
        public int first_point;
        public int sparsing_factor;
        public int segment_no;
        public int subarray_count;
        public int sweeps_per_acq;
        public short points_per_pair;
        public short pair_offset;
        public float vertical_gain;
        public float vertical_offset;
        public float max_value;
        public float min_value;
        public short nominal_bits;
        public short nom_subarray_count;
        public float horizontal_interval;
        public double horizontal_offset;
        public double pixel_offset;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x30)]
        public string vertunit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x30)]
        public string horunit;
        public float horiz_uncertainty;
        public RtTimeOld trigger_time;
        public float acq_duration;
        public short ca_record_type;
        public short processing_done;
        public short reserved5;
        public short ris_sweeps;
        public short time_base;
        public short vertical_coupling;
        public float probe_attenuation;
        public short fixed_vertical_gain;
        public short band_width_limit;
        public float vertical_vernier;
        public float acquisition_vertical_offset;
        public short wave_source;
    }
}

