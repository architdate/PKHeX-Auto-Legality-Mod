from lxml import etree
from io import StringIO
import re
import os
import sys


ns = {'d': 'http://schemas.microsoft.com/developer/msbuild/2003'}
tree = etree.parse("PKHeX.WinForms.csproj")
# Check if this project has already been patched
if tree.find("/d:ItemGroup//d:Compile[@Include='AutoLegality\\ArchitMod.cs']", ns) is not None:
    print("Already patched, aborting!")
    sys.exit(1)

ig = tree.find("/d:ItemGroup//d:Compile/..", ns)

mods = ["AutoLegality", "PGLRentalLegality", "MGDBDownloader"]
new_files = []
inbuilt_references = ["System.IO.Compression.FileSystem", "System.Numerics"]
custom_references = {"BouncyCastle.CryptoExt":"..\\..\\PKHeX-Auto-Legality-Mod\\Addons (Optional)\\PGL QR Auto Legality\\BouncyCastle.CryptoExt.dll"}
user_controls = ["AutoLegality\\ArchitMod.cs", "AutoLegality\\PKSMAutoLegality.cs"]
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
    child = etree.SubElement(ig, "Compile")
    child.set("Include", file)
    if file in user_controls:
        child2 = etree.SubElement(child, "SubType")
        child2.text = "UserControl"
    if file in forms:
        child2 = etree.SubElement(child, "SubType")
        child2.text = "Form"
    if file == "AutoLegality\AutoLegality.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "AutoLegality.cs"
    if file == "MGDBDownloader\MGDBDownloader.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "MGDBDownloader.cs"
    if file == "PGLRentalLegality\PGLRentalLegality.Designer.cs":
        child2 = etree.SubElement(child, "DependentUpon")
        child2.text = "PGLRentalLegality.cs"

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

tree.write("PKHeX.WinForms.csproj", pretty_print=True)

with open("MainWindow/Main.Designer.cs", "r+", encoding="utf-8") as fp:
    data=fp.read()
    prog = re.compile("this.Menu_Showdown.DropDownItems.AddRange\(.*{.*}\);\\n", re.DOTALL)
    m = prog.search(data)
    modified = data[:m.end()] + "            this.Menu_Showdown.DropDownItems.Add(EnableAutoLegality(resources));\n            this.Menu_Showdown.DropDownItems.Add(EnablePGLRentalLegality(resources));\n            this.Menu_Showdown.DropDownItems.Add(EnableMGDBDownloader(resources));\n" + data[m.end():]
    fp.seek(0)
    fp.truncate()
    fp.write(modified)

print("All files patched.")