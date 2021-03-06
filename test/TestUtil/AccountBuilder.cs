using Chromia.Postchain.Ft3;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class AccountBuilder
{
    private Blockchain _blockchain;
    private User _user;
    private int _balance = -1;
    private Asset _asset;
    private List<KeyPair> _participants = new List<KeyPair>(){new KeyPair()};
    private int _requiredSignaturesCount = 1;
    private List<FlagsType> _flags = new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer};
    private int _points = 0;


    public AccountBuilder(Blockchain blockchain, User user)
    {
        if(user == null)
        {
            user = TestUser.SingleSig();
        }

        this._blockchain = blockchain;
        this._participants = new List<KeyPair>(){user.KeyPair};
        this._user = user;
    }

    /* Public functions */

    public static AccountBuilder CreateAccountBuilder(Blockchain blockchain, User user = null)
    {
        return new AccountBuilder(blockchain, user);
    }

    public AccountBuilder WithAuthFlags(IEnumerable<FlagsType> flags)
    {
        this._flags = flags.ToList();
        return this;
    }

    public AccountBuilder WithParticipants(IEnumerable<KeyPair> participants)
    {
        this._participants = participants.ToList();
        return this;
    }

    public AccountBuilder WithBalance(Asset asset, int balance)
    {
        this._asset = asset;
        this._balance = balance;
        return this;
    }

    public AccountBuilder WithPoints(int points)
    {
        this._points = points;
        return this;
    }

    public AccountBuilder WithRequiredSignatures(int count)
    {
        this._requiredSignaturesCount = count;
        return this;
    }

    public async Task<Account> Build()
    {
        var account = await this.RegisterAccount();
        if(account != null)
        {
             await this.AddBalanceIfNeeded(account);
            account.RateLimit = await this.AddPointsIfNeeded(account);
        }
       
        return account;
    }

     /* Private functions */

    private async Task<Account> RegisterAccount()
    {
        return await Account.Register(
            this.GetAuthDescriptor(),
            this._blockchain.NewSession(this._user)
        );
    }

    private async Task AddBalanceIfNeeded(Account account)
    {
        if(this._asset != null && this._balance != -1)
        {
            await AssetBalance.GiveBalance(account.Id, this._asset.GetId(), this._balance, this._blockchain);
        }
    }

    private async Task<RateLimit> AddPointsIfNeeded(Account account)
    {
        if(this._points > 0)
        {
            await RateLimit.GivePoints(account.Id, this._points, this._blockchain);
        }
        return await RateLimit.GetByAccountRateLimit(account.Id, this._blockchain);
    }

    private AuthDescriptor GetAuthDescriptor()
    {
        if(this._requiredSignaturesCount > this._participants.Count)
        {
            throw new System.Exception("Number of required signatures has to be less than number of participants");
        }

        if(this._participants.Count > 1)
        {
            var participants = new List<byte[]>();
            foreach (var participant in this._participants)
            {
                participants.Add(participant.PubKey);
            }

            return new MultiSignatureAuthDescriptor(
                participants,
                this._requiredSignaturesCount,
                this._flags.ToArray(),
                this._user.AuthDescriptor.Rule
            );
        }
        else
        {
            return new SingleSignatureAuthDescriptor(
                this._participants[0].PubKey,
                this._flags.ToArray(),
                this._user.AuthDescriptor.Rule
                );
        }
    }
}