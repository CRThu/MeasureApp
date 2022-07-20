clear all;

%fs = 16000000/16;                                   %ADC����Ƶ��28000000/16 16000000/32
fs = 200000;                                   %ADC����Ƶ��28000000/16 16000000/32
bitsize = 16;                                       %ADλ��
maxdata = 2^bitsize-1;                              %12λADC�������룬��AD��λ���й�
VREF = 10.0 ;%�ο���ѹ2.5V3.298V3.253
VIN = 9.530;%ʵ�������ѹ
Voffset =0;


filename = 'AD7606_TestData_88d69.txt';             %'File_AdDataConvert.txt';File_AdDataConvert

fid = fopen(filename);

data=[];

while ~feof(fid)%%%%%%��whileѭ����txt�е�ʮ������ת��ʮ����
 
m=fscanf(fid,'%4x',[1 inf]);%��ÿ3���ַ����1��ʮ������������1��N�о���m��

data=[data;m];         %�����������ݴ���data������

end

code = data';                           
code =code(1:16384);
code_len = length(code);                            %�������ݵ���Ŀ
%code_len = 2000;                            %�������ݵ���Ŀ
%code = code /maxdata;                              %��һ��
fpnum = fopen('temp.txt','wt');                     %����temp.txt
fprintf(fpnum,'%d\n',code);                         %��ת��ʮ���ƽ������д��temp.txt

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
% Dout=code-(2^bitsize-1)/2;                          %�����Ĳ���ֵת��Ϊ����ֵ


idx_start=strfind(filename,'VDD');
idx_end=strfind(filename,'V_ADC_result');
adc_condition=filename(idx_start:idx_end);%��ӡ��������
% s=sprintf(adc_condition);%��ӡ��������
% text(fs/60, +10, s);

% %-------------------------------------------------------------------------- 
df1 = fs/code_len;
%code_fft = fft (code.*blackmanharris(code_len));    %������ֵ��FFT�任,����blackmanharris������
%code_fft = fft (code(code_len).*blackmanharris(code_len));    %������ֵ��FFT�任,����blackmanharris������
code_fft = fft (code(1:code_len).*blackmanharris(code_len),code_len);    %������ֵ��FFT�任,����blackmanharris������
%code_fft = fft (code.*boxcar(code_len));
%%������ֵ��FFT�任,���Ӿ��δ���������������ϸ�֤fin=m*fs/n����Ҫ�Ӿ��δ�
%code_fft = fft (code.*rectwin(code_len));    %������ֵ��FFT�任,���Ӿ��δ�����


Dout_dB=20*log10(abs(code_fft));                    %��FFT�任����÷ֱ���ʾ
%Display the results in the frequency domain with an FFT plot
figure;
maxdB=max(Dout_dB(5:code_len/2));                   %�ҵ���ֱ�������������ֵ���õ���Ƶ�źŵķ�ֵ
%For TTIMD, use the following short routine, normalized to ��6.5dB full-scale.
%plot([0:numpt/2-1].*fclk/numpt,Dout_dB(1:numpt/2)-maxdB-6.5);
plot([0:code_len/2-1].*fs/code_len,Dout_dB(1:code_len/2)-maxdB,'-*');    %����ΪƵ�ʣ�����Ϊ��ֵ���ֱ���
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
fin=find(Dout_dB(1:code_len/2) == maxdB);           %��ÿһ��ķ�ֵ���Ƶ�źŵķ�ֵ�Ƚϣ��ҵ���Ƶ�źŵ�λ�á�
%Span of the input frequency on each side
span=max(round(code_len/1000),5);                    %����ÿһ�ߵķ�Χ��������ÿһ��ȡ�ĵ���
span=5;
%Approximate search span for harmonics on each side
spanh=2;                                            %г��ÿһ��ȡ�ĵ���
%Determine power spectrum
%spectP=(abs(code_fft)).*(abs(code_fft));            %�������
spectP=(abs(code_fft(1:code_len))).*(abs(code_fft(1:code_len)));            %�������
%Find DC offset power
Pdc=sum(spectP(1:span));                            %��һ���㵽��span����Ĺ��ʺ�Ϊֱ����������
%Extract overall signal power
Ps=sum(spectP(fin-span:fin+span));                  %��Ƶ�ź�λ�����span������ұ�span����Ĺ���֮��Ϊ��Ƶ����
%Vector/matrix to store both frequency and power of signal and harmonics
Fh=[];                                              %���ڴ洢������ÿ��г����Ƶ��
%The 1st element in the vector/matrix represents the signal, the next element represents
%the 2nd harmonic, etc.
Ph=[];                                              %���ڴ洢������ÿ��г���Ĺ���
%Find harmonic frequencies and power components in the FFT spectrum
for har_num=1:10
%Input tones greater than fSAMPLE are aliased back into the spectrum
tone=rem((har_num*(fin-1)+1)/code_len,1);
if tone>0.5                                         %������1/2����Ƶ�ʵ�г���޳�
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

har_peak=max(spectP(temp1:temp2));            %���г�����Ĺ���
har_bin=find(spectP(temp1:temp2)==har_peak);  %�������ʵ�λ�ã���Դ˴�г����һ�����λ�ã�
har_bin=har_bin+round(tone*code_len)-spanh-1;                                           %�����Ե�һ�����λ��

temp3 = har_bin-1;
temp4 = har_bin+1;

% if temp3<=0
%     temp3 = 1;
% end

Ph=[Ph sum(spectP(temp3:temp4))];                                               %���ÿ��г���Ĺ���


end
%Determine the total distortion power
Pd=sum(Ph(2:5));                                    %���ǰ5��г���Ĺ���
%Determine the noise power
Pn=sum(spectP(1:code_len/2))-Pdc-Ps-Pd;             %�����������
format;
%A=(max(code)-min(code))/2^12
%AdB=20*log10(A)
% SINAD=10*log10((Ps+Pn+Pd)/(Pn+Pd));
SINAD=10*log10((Ps)/(Pn+Pd));
SNR=10*log10(Ps/Pn);
disp('THD is calculated from 2nd through 5th order harmonics');
THD=10*log10(Pd/Ph(1));
SFDR=10*log10(Ph(1)/max(Ph(2:5)));%SFDR=10*log10(Ph(1)/max(Ph(2:10)));�û������Ե�2����10��г���Ļ�,���ڵ�9��г�������˻���λ�ã������SFDR����0
% ENOB = (SINAD-(10*log10(1.5)))/(20*log10(2));
ENOB = ((SINAD-(10*log10(1.5)))+20*log10(VREF/VIN))/(20*log10(2));%���ӷ�ֵ������ϵ��
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
