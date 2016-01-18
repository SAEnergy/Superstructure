<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="msxsl"
  xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
  xmlns:my="my:my"
  xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

    <xsl:output method="xml" indent="yes" />

    <xsl:strip-space elements="*"/>

    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

<xsl:template match="wix:Component[wix:File[@Source='$(var.SourceLocation)\HostService.exe']]">
    <xsl:copy>
        <xsl:apply-templates select="node() | @*" />
        <wix:ServiceInstall Id="ServiceInstaller" Account="LocalSystem" Description="!(loc.ServiceDescription)" DisplayName="!(loc.ServiceDisplayName)" ErrorControl="normal"
                LoadOrderGroup="NetworkProvider" Name="HostService" Start="disabled" Type="ownProcess" Vital="yes" />

        <wix:ServiceControl Id="ServiceControl" Name="HostService" Stop="both" Remove="uninstall" />
    </xsl:copy>
</xsl:template>
<xsl:template match="wix:Component[wix:File[@Source='$(var.SourceLocation)\HostService.exe.config']]">
    <xsl:copy>
        <xsl:apply-templates select="node() | @*" />
        <util:XmlFile
            Id="LogDirectory"
            Action="setValue"
            File="$(var.SourceLocation)HostService.exe.config"
            ElementPath="/configuration/appSettings/add[\[]@key='LogDirectory'[\]]"
            Name="value"
            Value="[WIXUI_LOGFILEFOLDER]"
            Permanent="yes" 
            Sequence="1" />

        <util:XmlFile
           Id="MaxLogFileSize"
           Action="setValue"
           File="$(var.SourceLocation)HostService.exe.config"
           ElementPath="/configuration/appSettings/add[\[]@key='MaxLogFileSize'[\]]"
           Name="value"
           Value="[WIXUI_LOGFILESIZE]"
           Permanent="yes" 
           Sequence="1" />

        <util:XmlFile
            Id="MaxLogFileCount"
            Action="setValue"
            File="$(var.SourceLocation)HostService.exe.config"
            ElementPath="/configuration/appSettings/add[\[]@key='MaxLogFileCount'[\]]"
            Name="value"
            Value="[WIXUI_LOGFILECOUNT]"
            Permanent="yes" 
            Sequence="1" />

        <util:XmlFile
            Id="LogFilePrefix"
            Action="setValue"
            File="$(var.SourceLocation)HostService.exe.config"
            ElementPath="/configuration/appSettings/add[\[]@key='LogFilePrefix'[\]]"
            Name="value"
            Value="[WIXUI_LOGFILEPREFIX]"
            Permanent="yes" 
            Sequence="1" />

        <util:XmlFile
            Id="PortNumber"
            Action="setValue"
            File="$(var.SourceLocation)HostService.exe.config"
            ElementPath="/configuration/appSettings/add[\[]@key='PortNumber'[\]]"
            Name="value"
            Value="[WIXUI_SERVERPORT]"
            Permanent="yes" 
            Sequence="1" />
    </xsl:copy>
</xsl:template>

    <xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.vshost.exe')]" use="@Id" />

    <xsl:template match="wix:Component[key('service-search', @Id)]" />
    <xsl:template match="wix:ComponentRef[key('service-search', @Id)]" />

</xsl:stylesheet>