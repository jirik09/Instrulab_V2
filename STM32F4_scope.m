% Support m-file for STM32F4 dicovery - oscilloscope
% Autor: Y3588231
% to run one section please click to it and press ctrl + enter
%% Establish connection on serial port 
% change number of serial port
port='COM19'
s = serial(port, 'BaudRate', 230400, 'InputBufferSize', 1194304);
fopen(s);

%% Close serial port
fclose(s);


%%
fopen(s);

%% Identify the instrument
flushinput(s);
fwrite(s,'IDN?;');
pause(0.2);
if s.BytesAvailable<10 
    disp(['Device is not connected to port: ', port]);
else
    data = fread(s,s.BytesAvailable);
    str=char(data');
    disp(str);
end
%%
flushinput(s);

fwrite(s,'GEN_:STRT;');
s.BytesAvailable
%%
data = fread(s,4);
str=char(data');
disp(str)
%% Yours settings
flushinput(s);
fwrite(s,'OSCP:STRT;');
pause(0.1)
total=0;
while s.BytesAvailable>0
    tmp=s.BytesAvailable;
    total=total+tmp
    data=fread(s,tmp);
    str=char(data');
    disp(str)
    pause(0.5);
end
% pause(3);
% fwrite(s,'OSCP:STOP;');
data = fread(s,4);
str=char(data');
disp(str)

%%
data = fread(s,400);
data8 = uint8(data);
dataType = typecast(data8,'uint16');
plot(dataType)

%% 12Bits
flushinput(s);
fwrite(s,'OSCP:STRT;');
pause(1)
s.BytesAvailable
data=fread(s,228);
disp(char(data(1:28)'))
data8=uint8(data(29:end));
datasmp = typecast(data8,'uint16');
plot(datasmp)
datasmp(50)
datasmp(51)

%% 8 bits
flushinput(s);
fwrite(s,'OSCP:STRT;');
pause(1)
s.BytesAvailable
data=fread(s,128);
disp(char(data(1:28)'))
data8=uint8(data(29:end));
datasmp = typecast(data8,'uint8');
plot(datasmp)
datasmp(50)
datasmp(51)

%% 
flushinput(s);
fwrite(s,'OSCP:DATA 12B_;'); %RISE, FALL
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)
%% 
flushinput(s);
fwrite(s,'OSCP:TRIG NORM;'); %RISE, FALL
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)

%% Yours settings
flushinput(s);
fwrite(s,'OSCP:DATA 8B__;');
fwrite(s,'OSCP:CHAN 2CH_;');
fwrite(s,'OSCP:LENG 20K_;');
fwrite(s,'OSCP:FREQ 2M__;');
fwrite(s,'OSCP:TRCH 1CH_;');
pause(0.1);
data = fread(s,s.BytesAvailable);
str=char(data');
disp(str)

%%
flushinput(s);
fwrite(s,'OSCP:STRT;');
pause(1)
s.BytesAvailable
data = fread(s,s.BytesAvailable);
data8=uint8(data(29:end-4));
datasmp = typecast(data8,'uint8');
plot(datasmp)


%% (Optional) Settings of prettriger
pretriggerValue = 0.5; %double from 0 to 1

flushinput(s);
fwrite(s,'OSCP:PRET ');
fwrite(s,ceil(pretriggerValue*65535),'uint32');
fwrite(s,';')
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)

%% (Optional) Settings of trigger level
triggerLevel = 0.5; %double from 0 to 1 (leave some margin)

flushinput(s);
fwrite(s,'GEN_:LENG ');
fwrite(s,ceil(100),'uint32');
fwrite(s,';')
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)

%% (Optional) Settings of trigger edge
flushinput(s);
fwrite(s,'GEN_:CHAN 1CH_;'); %RISE, FALL
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)

%% (Optional) Settings of trigger level
triggerLevel = 0.5; %double from 0 to 1 (leave some margin)

flushinput(s);
fwrite(s,'GEN_:DATA ');
fwrite(s,ceil(0),'uint16');%index
fwrite(s,ceil(20),'uint8');%length
fwrite(s,ceil(1),'uint8');%channel
fwrite(s,':')
fwrite(s,ceil(50*4),'uint16');
fwrite(s,ceil(100*4),'uint16');
fwrite(s,ceil(150*4),'uint16');
fwrite(s,ceil(200*4),'uint16');
fwrite(s,ceil(250*4),'uint16');
fwrite(s,ceil(300*4),'uint16');
fwrite(s,ceil(350*4),'uint16');
fwrite(s,ceil(400*4),'uint16');
fwrite(s,ceil(450*4),'uint16');
fwrite(s,ceil(500*4),'uint16');
fwrite(s,ceil(550*4),'uint16');
fwrite(s,ceil(600*4),'uint16');
fwrite(s,ceil(650*4),'uint16');
fwrite(s,ceil(700*4),'uint16');
fwrite(s,ceil(750*4),'uint16');
fwrite(s,ceil(800*4),'uint16');
fwrite(s,ceil(850*4),'uint16');
fwrite(s,ceil(900*4),'uint16');
fwrite(s,ceil(950*4),'uint16');
fwrite(s,ceil(1000*4),'uint16');
fwrite(s,';')
pause(0.1);
data = fread(s,4);
str=char(data');
disp(str)

%% Capture data in SINGLE mode
flushinput(s);

pause(1);
fwrite(s,'OSCP:TRIG SING:FREQ 10K_:LENG 500_;');
fwrite(s,'OSCP:STRT;');

% waiting and display response from scope
pause(0.1); 
data = fread(s,8);
str=char(data');
disp(str)

while s.BytesAvailable<4
  pause(0.1);
end
  
%waiting for data
while ~strcmp(char((fread(s,4))'),'DATA')
  pause(0.01);
end
    
%read header value (resolution and data length)
data = fread(s,4);
data8 = uint8(data);
dataType = typecast(data8,'uint16');
toRead=double(dataType(2));
if dataType(1)>256 %if resolution > 8bit --> data is twice as length
  toRead=toRead*2;
end
   
%wait while all data is send
while s.BytesAvailable<toRead
  pause(0.1);
end
data = fread(s,double(toRead));
    
%if resolution > 8bit --> combine data to uint16
if dataType(1)>256
  data8 = uint8(data);
  data = typecast(data8,'uint16');
end
    
%plot data
plot(data,'g');
title('Sampled Data');
grid on
axis([0 dataType(2) 0 dataType(1)]);
xlabel('Samples');
ylabel('ADC value');

%% Capture data in AUTO or NORMAL mode
numberOfAcquisitions = 10;

flushinput(s);
clear dataPlot;
%pause(1);

%fwrite(s,'OSCP:TRIG AUTO:FREQ 20K_:LENG 1K__;');
fwrite(s,'OSCP:TRIG AUTO;');
fwrite(s,'OSCP:STRT;');

% waiting and display response from scope
pause(0.1); 
data = fread(s,8);
str=char(data');
disp(str)

for ii=1:numberOfAcquisitions
  %waiting for data
  while ~strcmp(char((fread(s,4))'),'TRIG')
    pause(0.01);
  end
  
  while s.BytesAvailable<16
    pause(0.01);
  end
    
 
  actualChan=0;
  totalChan=1;
  
  while actualChan<totalChan
    %read header value (resolution and data length)
    data = fread(s,16);
    data8 = uint8(data);
    dataType = typecast(data8,'uint16');
    toRead=double(dataType(6));
    adcRes=double(dataType(5));
    
    actualChan=mod(double(dataType(8)),256);
    totalChan=double(dataType(8)/256);    
    
    %wait while all data is send
    while s.BytesAvailable<toRead
        pause(0.1);
    end
    data= fread(s,double(toRead*2));

    if adcRes>256
    data8 = uint8(data);
    data = typecast(data8,'uint16');
    end
    
    dataPlot(actualChan,:)=data;
  

  end
  plot(dataPlot');
  

   title(['Sampled Data', num2str(ii)]);
  grid on
  axis([0 toRead 0 adcRes]);
  xlabel('Samples');
  ylabel('ADC value');
  pause(0.05);
  fwrite(s,'OSCP:NEXT;');
    
end

fwrite(s,'OSCP:STOP;');

%% Close serial port
fclose(s);



