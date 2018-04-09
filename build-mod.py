from lxml import etree
from io import StringIO
import re
import os
import sys


def getopts(argv):
    opts = {}  # Empty dictionary to store key-value pairs.
    while argv:  # While there are arguments left to parse...
        if argv[0][0] == '-':  # Found a "-name value" pair.
            opts[argv[0]] = argv[1]  # Add key and value to the dictionary.
        argv = argv[1:]  # Reduce the argument list by copying it starting from index 1.
    return opts


cmdargs = getopts(sys.argv)
latestCommit = False
if '-c' in cmdargs:
    if str(cmdargs['-c']).lower().strip() == "true":
        latestCommit = True

ns = {'d': 'http://schemas.microsoft.com/developer/msbuild/2003'}
tree = etree.parse("PKHeX.WinForms.csproj")
# Check if this project has already been patched
if tree.find("/d:ItemGroup//d:Compile[@Include='AutoLegality\\ArchitMod.cs']", ns) is not None:
    print("Already patched, aborting!")
    sys.exit(1)

ig = tree.find("/d:ItemGroup//d:Compile/..", ns)

mods = ["AutoLegality", "PGLRentalLegality", "MGDBDownloader", "URLGenning"]
new_files = []
resource_files = ["AutoLegality\\AutoLegalityMod.resx", "AutoLegality\\AutoLegalityMod.Designer.cs"]
image_files = ["AutoLegality\\Resources\\img\\menuautolegality.png", "AutoLegality\\Resources\\img\\autolegalitymod.png", "AutoLegality\\Resources\\img\\mgdbdownload.png", "AutoLegality\\Resources\\img\\pglqrcode.png", "AutoLegality\\Resources\\img\\urlimport.png"]
inbuilt_references = ["System.IO.Compression.FileSystem", "System.Numerics"]
custom_references = {"BouncyCastle.CryptoExt":"..\\..\\PKHeX-Auto-Legality-Mod\\Addons (Optional)\\PGL QR Auto Legality\\BouncyCastle.CryptoExt.dll"}
user_controls = ["AutoLegality\\ArchitMod.cs", "AutoLegality\\PKSMAutoLegality.cs", "URLGenning\\URLGenning.cs"]
forms = ["AutoLegality\\AutoLegality.cs", "MGDBDownloader\\MGDBDownloader.cs", "PGLRentalLegality\\PGLRentalLegality.cs"]
embedded_resources = ["AutoLegality\\Resources\\text\\evolutions.txt", "PGLRentalLegality\\Resources\\text\\pokemonAbilities.csv", "PGLRentalLegality\\Resources\\text\\pokemonFormAbilities.csv"]

for mod in mods:
    for root, dirs, files in os.walk(mod):
        for name in files:
            fullpath = os.path.join(root, name)
            if not os.path.isdir(fullpath):
                new_files.append(os.path.join(root, name))
        for name in dirs:
            fullpath = os.path.join(root, name)
            if not os.path.isdir(fullpath):
                new_files.append(os.path.join(root, name))

for file in new_files:
    if file in embedded_resources:
        continue
    if file in resource_files:
        continue
    if file in image_files:
        continue
    child = etree.SubElement(ig, "Compile")
    child.set("Include", file)
    if file in user_controls:
        child2 = etree.SubElement(child, "SubType")
        child2.text = "UserControl"
    if file in forms:
        child2 = etree.SubElement(child, "SubType")
        child2.text = "Form"
    if file == "AutoLegality\\AutoLegality.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "AutoLegality.cs"
    if file == "MGDBDownloader\\MGDBDownloader.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "MGDBDownloader.cs"
    if file == "PGLRentalLegality\\PGLRentalLegality.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "PGLRentalLegality.cs"
    if file == "URLGenning\\URLGenning.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "URLGenning.cs"

for file in image_files:
    child = etree.SubElement(tree.find("/d:ItemGroup//d:Content/..", ns), "Content")
    child.set("Include", file)

ig3 = tree.find("/d:ItemGroup//d:Reference/..", ns)
for file in inbuilt_references:
    child = etree.SubElement(ig3, "Reference")
    child.set("Include", file)
for key, value in custom_references.items():
    child = etree.SubElement(ig3, "Reference")
    child.set("Include", key)
    child2 = etree.SubElement(child, "HintPath")
    child2.text = value

ig2 = tree.find("/d:ItemGroup//d:Content/..", ns)
for file in embedded_resources:
    child = etree.SubElement(ig2, "EmbeddedResource")
    child.set("Include", file)

# Setting resources

child = etree.SubElement(ig, "Compile")
child.set("Include", "AutoLegality\\AutoLegalityMod.Designer.cs")
child2 = etree.SubElement(child, "AutoGen")
child2.text = "True"
child3 = etree.SubElement(child, "DesignTime")
child3.text = "True"
child4 = etree.SubElement(child, "DependentUpon")
child4.text = "AutoLegalityMod.resx"
childB = etree.SubElement(ig, "EmbeddedResource")
childB.set("Include", "AutoLegality\\AutoLegalityMod.resx")
childB2 = etree.SubElement(childB, "Generator")
childB2.text = "ResXFileCodeGenerator"
childB3 = etree.SubElement(childB, "LastGenOutput")
childB3.text = "AutoLegalityMod.Designer.cs"


tree.write("PKHeX.WinForms.csproj", pretty_print=True)

with open("MainWindow/Main.Designer.cs", "r+", encoding="utf-8") as fp:
    data=fp.read()
    prog = re.compile("this.Menu_Showdown.DropDownItems.AddRange\(.*{.*}\);\\n", re.DOTALL)
    m = prog.search(data)
    modified = data[:m.end()] + "            this.Menu_Tools.DropDownItems.Insert(0, EnableMenu(resources));\n            this.Menu_AutoLegality.DropDownItems.Add(EnableAutoLegality(resources));\n            this.Menu_AutoLegality.DropDownItems.Add(EnablePGLRentalLegality(resources));\n            this.Menu_AutoLegality.DropDownItems.Add(EnableMGDBDownloader(resources, {0}));\n            this.Menu_AutoLegality.DropDownItems.Add(EnableURLGenning(resources));\n".format(str(latestCommit).lower()) + data[m.end():]    
    fp.seek(0)
    fp.truncate()
    fp.write(modified)

print("All files patched.")