using System;

namespace YuzuDelivery.Umbraco.BlockList.Tests.Stubs;

public class ConsistentGuidFactoryStub : GuidFactory
{
    private readonly Guid[] _guids = {
        #region guid spam
        new ("187280bc-8dbb-4e99-b603-53eb20f8dad6"),
        new ("238d5fd9-a130-4080-b56c-d3902af148f8"),
        new ("b6ba415a-765a-4770-a7e9-66fd180c994b"),
        new ("9cb07547-f3fd-4bb4-9eeb-e02c66bd834a"),
        new ("61df46f0-d4bd-4190-8477-a65214ca0eef"),
        new ("8fc4965a-c9b4-4fdc-8608-52a8de6e9252"),
        new ("6ca50244-4a18-4116-aa75-2ba20f8eaa61"),
        new ("14b06a2b-6589-4ce9-915b-2bc1dce03d8e"),
        new ("de569061-d4c8-441c-8d99-9c87d052c10f"),
        new ("c31e2d25-7c67-4eb1-95b9-d48781859b25"),
        new ("18d4ace4-344b-4350-ae91-3caf92723971"),
        new ("a8bd981e-090c-4a1a-8b77-2f27625bcf6f"),
        new ("c2661638-9d73-4bfd-8592-d6d0180623a8"),
        new ("4eaf9bb6-42e0-4c11-b9fb-899c98fe0293"),
        new ("8ca4a3da-6763-42c8-8f11-3a2dcfd0f6aa"),
        new ("79bc4fa7-f48f-4fe2-a4f8-234606546ef1"),
        new ("f392f522-b91d-4739-a677-4414059280d1"),
        new ("a74352e9-7a44-47fe-bad1-909c7abffb1c"),
        new ("50ebe356-0291-480b-b2f9-25cdbdbd430f"),
        new ("d08725fc-8135-4634-920b-cb6ec6e9533f"),
        new ("13bf4ab5-2910-4921-8eec-699b667ec82d"),
        new ("2f50802e-df34-45ea-b81d-7d727763b308"),
        new ("4e9f1de3-34b1-41ef-bf7a-cc684d1c119a"),
        new ("ae581040-997b-4f55-bcef-504005b1d9f5"),
        new ("6407fe1a-4736-4228-a9b8-957e5d7803fa"),
        new ("0bc19752-36c4-45a8-a974-e9f7938b9f87"),
        new ("2e7f9fb8-0ae9-4c02-8a51-e8e165e4bf50"),
        new ("9460e325-a380-49ad-a1b7-3feb6e49d01b"),
        new ("590b0e30-49db-4ea5-abbd-4f962dfeb19a"),
        new ("a1900158-15a6-4af7-8892-7d9ff6580f4d"),
        new ("e2654747-df4e-48ac-9d02-1b229af8fc18"),
        new ("d06c3356-6247-4669-a27c-2f8becfef2bb"),
        new ("cd103014-457a-4edc-93bc-b34d68cd9de0"),
        new ("6f8b7434-c867-4eb2-886c-e201b0534cc4"),
        new ("ae30772b-6d84-4aa8-a357-7b6eff63c7bf"),
        new ("b7557217-5d10-4e3a-bcb8-0f5d4a3420b8"),
        new ("5ac16fa5-661b-4d12-a47a-0540d0e27637"),
        new ("bf40ceb7-4f60-4ced-a9da-77a764e6bd36"),
        new ("45c3fed3-7566-4cf7-a316-91ef6f6cebf9"),
        new ("e672dd08-d702-428a-acf5-0ffde660f276"),
        new ("2394e57e-de1a-4376-8833-b0e0f95bcffa"),
        new ("b6e9a822-13aa-48c7-ad01-4e4e36dcf915"),
        new ("3893fbf3-d850-485c-aba2-d3c91c464cc6"),
        new ("e086afa0-a4b7-4858-ba8a-6817a1501da5"),
        new ("3aaa330f-bedd-4138-b4e4-ae6b8cd2b940"),
        new ("1cd3bdce-cc4c-4998-a748-69e924194399"),
        new ("f2971089-dfff-4820-8b87-16108300f4ce"),
        new ("30380950-9d05-4c89-a303-cbd634897232"),
        new ("0c7870a5-8cc0-4af4-b5f8-01e5ad877878"),
        new ("1b16f996-177c-48c6-885b-538ec1b1e877"),
        new ("1b1ad2c5-6bf0-4331-adb8-d3abcf436609"),
        new ("95f47c48-3e61-427c-a2e7-3880627160fa"),
        new ("4a7fd7ca-3e1a-440e-a038-a6abc1c2b87b"),
        new ("60089944-e756-492e-b3d9-f2c88542dbfe"),
        new ("9bb2240a-2508-4fa4-87d7-b852c6adfb84"),
        new ("1b56b0ea-7e7d-48c6-bd39-0191a1c135c1"),
        new ("b8389e02-0e9c-47ee-a1de-31b8de30e037"),
        new ("710317e4-6e72-4ae9-9a81-d688ba09d8d2"),
        new ("e46c97df-9484-4a61-9dd7-e8213492cf2e"),
        new ("646498bc-9e17-473d-b343-24a0b80494d1"),
        new ("b02226fa-d6cf-4cab-9c62-3fe0bbd10d6b"),
        new ("8359fe85-e506-4384-8316-96a6a238939b"),
        new ("308e1c2a-0d6d-482c-ad57-cccb0c268573"),
        new ("b155e28d-ab54-4dd8-a49d-8b40e81cacd9"),
        new ("8f6dde1e-ecd9-475a-b7f9-1c36b7db5f80"),
        new ("d6a9413b-da88-4f65-a9ac-7c1484d4e3c8"),
        new ("fe475c84-259a-43a5-bb2b-8a03b433d865"),
        new ("d47c41d0-a844-4ba1-a5f1-4a81fdf0bb9d"),
        new ("af695a79-601d-4e94-8c3c-7a830dcb3268"),
        new ("132dccfd-7796-4e51-bcc6-60f28ceb8c74"),
        new ("ab917a04-4c75-4340-9a22-d48f271fc99e"),
        new ("190e70a7-443c-4a5f-900f-c486ca617959"),
        new ("0a46e8b1-775e-481a-8ffa-398a73e312ca"),
        new ("423091bd-07b6-46ad-9dc9-7673cc23cc4c"),
        new ("623dda8d-cdd3-4f02-80a1-bccf9ed5bfd0"),
        new ("e9077a53-c49d-45cf-996b-5bf0f85739c2"),
        new ("6c24f959-a12f-4851-a5b7-a73693038ad1"),
        new ("0bd82da3-b1fc-4e8b-86bb-3d1d8c8c5c88"),
        new ("e034f148-25e4-4122-b5e0-857587b5c333"),
        new ("ec87da6a-b2ab-446a-9a5d-6bda72320402"),
        new ("a0dded88-b1f0-4a97-8dd0-1bd14065cac9"),
        new ("fae06fad-8750-46e1-94ce-08a6b4b89652"),
        new ("b865ba25-38b9-4202-85c8-7cf2fa6fedde"),
        new ("af261d30-9680-47d4-b85a-8ee84e523565"),
        new ("0a8fa938-d4e8-4386-9671-9b4e4e42d712"),
        new ("bf320453-06df-47c7-b376-8eb3fbeb2014"),
        new ("59ddb90d-5090-4faf-a316-dc3fd30e1c87"),
        new ("a3e81e42-4a0d-4bd1-b79b-e6502adb3527"),
        new ("318a5626-a7b7-4399-9640-f6340f2aa298"),
        new ("ea8fd81b-44de-49f3-bc36-dc021e61d015"),
        new ("6f63c10c-96b5-4c18-879c-d3f087855285"),
        new ("423d6804-b025-4d2a-807b-cb3534902518"),
        new ("4065d2ce-e603-4981-b565-018eaaee4ab0"),
        new ("7832e4df-5c7f-4070-a552-d7c6a69777fc"),
        new ("8246d419-20f7-484d-b7a4-e1b827865080"),
        new ("04531fa0-5e86-476f-a7b5-a5a17ae18be2"),
        new ("c521055d-2775-428e-82e1-dfc08b231bdf"),
        new ("ac09fa86-4076-44cb-8d3b-da3ae78546d8"),
        new ("b89da275-d3d8-4c60-b0f1-1b4dbf1268a7"),
        new ("d8538bda-9b69-448a-add0-aaeebc8fa5d0"),
        #endregion
    };

    private int _next;

    public override Guid CreateNew()
    {
        if (_next >= _guids.Length)
        {
            throw new InvalidOperationException("Ran out of guids");
        }

        return _guids[_next++];
    }

    public override Guid CreateNew(Guid key)
    {
        if (_next >= _guids.Length)
        {
            throw new InvalidOperationException("Ran out of guids");
        }

        return _guids[_next++];
    }
}
