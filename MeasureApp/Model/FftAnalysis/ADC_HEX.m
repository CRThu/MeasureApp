clear all;

%fs = 16000000/16;                                   %ADC采样频率28000000/16 16000000/32
fs = 200000;                                   %ADC采样频率28000000/16 16000000/32
bitsize = 16;                                       %AD位数
maxdata = 2^bitsize-1;                              %12位ADC的最大代码，跟AD的位数有关
VREF = 10.0 ;%参考电压2.5V3.298V3.253
VIN = 9.530;%实际输入电压
Voffset =0;


filename = 'AD7606_TestData_88d69.txt';             %'File_AdDataConvert.txt';File_AdDataConvert

fid = fopen(filename);

data=[];

while ~feof(fid)%%%%%%此while循环将txt中的十六进制转成十进制
 
m=fscanf(fid,'%4x',[1 inf]);%按每3个字符组成1个十六进制数读到1行N列矩阵m中

data=[data;m];         %将读出的数据存入data矩阵中

end

code = data';                           
code =code(1:30000);
code_len = length(code);                            %波形数据点数目
%code_len = 2000;                            %波形数据点数目
%code = code /maxdata;                              %归一化
fpnum = fopen('temp.txt','wt');                     %创建temp.txt
fprintf(fpnum,'%d\n',code);                         %将转换十进制结果按行写入temp.txt

%Display a warning, when the input generates a code greater than full-scale
if (max(code)==2^bitsize-1) || (min(code)==0);
disp('Warning: ADC may be clipping!!!');
end
%Plot results in the time domain
figure;
%plot([1:code_len],code(1:code_len));
%plot([1:code_len],code);
plot([1:code_len],Voffset-VIN/2+code*VREF/(2^bitsize-1),'-*');
title('TIME DOMAIN')
xlabel('SAMPLES');
ylabel('DIGITAL OUTPUT CODE');

figure;
plot([-99:100],Voffset-VIN/2+code([(code_len-99):code_len 1:100])*VREF/(2^bitsize-1),'-*');
% code  = code+2048;
% %Recenter the digital sine wave
% Dout=code-(2^bitsize-1)/2;                          %将正的采样值转化为正负值


idx_start=strfind(filename,'VDD');
idx_end=strfind(filename,'V_ADC_result');
adc_condition=filename(idx_start:idx_end);%打印测试条件
% s=sprintf(adc_condition);%打印测试条件
% text(fs/60, +10, s);

% %-------------------------------------------------------------------------- 
df1 = fs/code_len;
%code_fft = fft (code.*blackmanharris(code_len));    %将采样值做FFT变换,并加blackmanharris窗函数
%code_fft = fft (code(code_len).*blackmanharris(code_len));    %将采样值做FFT变换,并加blackmanharris窗函数
code_fft = fft (code(1:code_len).*blackmanharris(code_len),code_len);    %将采样值做FFT变换,并加blackmanharris窗函数
%code_fft = fft (code.*boxcar(code_len));
%%将采样值做FFT变换,并加矩形窗函数，如果不能严格保证fin=m*fs/n，则不要加矩形窗
%code_fft = fft (code.*rectwin(code_len));    %将采样值做FFT变换,并加矩形窗函数


Dout_dB=20*log10(abs(code_fft));                    %将FFT变换结果用分贝表示
%Display the results in the frequency domain with an FFT plot
figure;
maxdB=max(Dout_dB(5:code_len/2));                   %找到除直流分量外的最大幅值，得到基频信号的幅值
%For TTIMD, use the following short routine, normalized to —6.5dB full-scale.
%plot([0:numpt/2-1].*fclk/numpt,Dout_dB(1:numpt/2)-maxdB-6.5);
plot([0:code_len/2-1].*fs/code_len,Dout_dB(1:code_len/2)-maxdB,'-*');    %横轴为频率，纵轴为幅值（分贝）
grid on;

str_title = 'FFT PLOT';
str_title = strcat(str_title,'(');
str_title = strcat(str_title,adc_condition);
str_title = strcat(str_title,')');
title(str_title);
xlabel('ANALOG INPUT FREQUENCY (kHz)');
ylabel('AMPLITUDE (dB)');

%Calculate SNR, SINAD, THD and SFDR values
%Find the signal bin number, DC = bin 1
fin=find(Dout_dB(1:code_len/2) == maxdB);           %将每一点的幅值与基频信号的幅值比较，找到基频信号的位置。
%Span of the input frequency on each side
span=max(round(code_len/1000),5);                    %基波每一边的范围，即基波每一边取的点数
span=5;
%Approximate search span for harmonics on each side
spanh=2;                                            %谐波每一边取的点数
%Determine power spectrum
%spectP=(abs(code_fft)).*(abs(code_fft));            %求出功率
spectP=(abs(code_fft(1:code_len))).*(abs(code_fft(1:code_len)));            %求出功率
%Find DC offset power
Pdc=sum(spectP(1:span));                            %第一个点到第span个点的功率和为直流分量功率
%Extract overall signal power
Ps=sum(spectP(fin-span:fin+span));                  %基频信号位置左边span个点和右边span个点的功率之和为基频功率
%Vector/matrix to store both frequency and power of signal and harmonics
Fh=[];                                              %用于存储基波和每次谐波的频率
%The 1st element in the vector/matrix represents the signal, the next element represents
%the 2nd harmonic, etc.
Ph=[];                                              %用于存储基波和每次谐波的功率
%Find harmonic frequencies and power components in the FFT spectrum
for har_num=1:10
%Input tones greater than fSAMPLE are aliased back into the spectrum
tone=rem((har_num*(fin-1)+1)/code_len,1);
if tone>0.5                                         %将大于1/2采样频率的谐波剔除
%Input tones greater than 0.5*fSAMPLE (after aliasing) are reflected
tone=1-tone;
end
Fh = [Fh tone];
%For this procedure to work, ensure the folded back high order harmonics do not overlap
%with DC or signal or lower order harmonics
temp1 = round(tone*code_len)-spanh;
temp2 = round(tone*code_len)+spanh;
% if temp1<=0
%     temp1 = 1;
% end

har_peak=max(spectP(temp1:temp2));            %求出谐波最大的功率
har_bin=find(spectP(temp1:temp2)==har_peak);  %求出最大功率的位置（相对此次谐波第一个点的位置）
har_bin=har_bin+round(tone*code_len)-spanh-1;                                           %求出相对第一个点的位置

temp3 = har_bin-1;
temp4 = har_bin+1;

% if temp3<=0
%     temp3 = 1;
% end

Ph=[Ph sum(spectP(temp3:temp4))];                                               %求出每次谐波的功率


end
%Determine the total distortion power
Pd=sum(Ph(2:5));                                    %求出前5次谐波的功率
%Determine the noise power
Pn=sum(spectP(1:code_len/2))-Pdc-Ps-Pd;             %求出噪声功率
format;
%A=(max(code)-min(code))/2^12
%AdB=20*log10(A)
% SINAD=10*log10((Ps+Pn+Pd)/(Pn+Pd));
SINAD=10*log10((Ps)/(Pn+Pd));
SNR=10*log10(Ps/Pn);
disp('THD is calculated from 2nd through 5th order harmonics');
THD=10*log10(Pd/Ph(1));
SFDR=10*log10(Ph(1)/max(Ph(2:5)));%SFDR=10*log10(Ph(1)/max(Ph(2:10)));用基波除以第2到第10次谐波的话,由于第9次谐波翻到了基波位置，算出来SFDR会是0
% ENOB = (SINAD-(10*log10(1.5)))/(20*log10(2));
ENOB = ((SINAD-(10*log10(1.5)))+20*log10(VREF/VIN))/(20*log10(2));%增加幅值的修正系数
disp(ENOB);
%Page 11 of 13
disp('Signal & Harmonic Power Components:');
HD=10*log10(Ph(1:10)/Ph(1));
%Distinguish all harmonics locations within the FFT plot
s=sprintf('                     SNR = %2.4f dB\n',SNR);
text(fs/12, -15, s);
s=sprintf('                     SINAD = %2.4f dB\n',SINAD);
text(fs/12, -25, s);
s=sprintf('                     THD = %2.4f dB\n',THD);
text(fs/12, -35, s);
s=sprintf('                     SFDR = %2.4f dB\n',SFDR);
text(fs/12, -45, s);
s=sprintf('                     ENOB = %2.4f bit\n',ENOB);
text(fs/12, -55, s);

disp(ENOB);

hold on;
plot(Fh(2)*fs,0,'mo',Fh(3)*fs,0,'cx',Fh(4)*fs,0,'r+',Fh(5)*fs,0,'g*',...
Fh(6)*fs,0,'bs',Fh(7)*fs,0,'bd',Fh(8)*fs,0,'kv',Fh(9)*fs,0,'y^');
legend('1st','2nd','3rd','4th','5th','6th','7th','8th','9th');
hold off;

%clear all;
